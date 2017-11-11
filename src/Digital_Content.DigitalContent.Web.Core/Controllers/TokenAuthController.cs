﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Abp;
using Abp.AspNetCore.Mvc.Authorization;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.MultiTenancy;
using Abp.Configuration;
using Abp.Extensions;
using Abp.Net.Mail;
using Abp.Notifications;
using Abp.Runtime.Caching;
using Abp.Runtime.Security;
using Abp.Runtime.Session;
using Abp.Timing;
using Abp.UI;
using Abp.Zero.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Digital_Content.DigitalContent.Authentication.TwoFactor.Google;
using Digital_Content.DigitalContent.Authorization;
using Digital_Content.DigitalContent.Authorization.Users;
using Digital_Content.DigitalContent.MultiTenancy;
using Digital_Content.DigitalContent.Web.Authentication.JwtBearer;
using Digital_Content.DigitalContent.Web.Authentication.TwoFactor;
using Digital_Content.DigitalContent.Web.Models.TokenAuth;
using Digital_Content.DigitalContent.Authorization.Impersonation;
using Digital_Content.DigitalContent.Identity;
using Digital_Content.DigitalContent.Notifications;
using Digital_Content.DigitalContent.Web.Authentication.External;
using Abp.Domain.Repositories;
using Digital_Content.DigitalContent.Authorization.Accounts.Dto;
using Digital_Content.DigitalContent.Authorization.Accounts;
using Digital_Content.DigitalContent.Web.Models.Tenant;

namespace Digital_Content.DigitalContent.Web.Controllers
{
    [Route("api/[controller]/[action]")]
    public class TokenAuthController : DigitalContentControllerBase
    {
        private const string UserIdentifierClaimType = "http://aspnetzero.com/claims/useridentifier";

        private readonly LogInManager _logInManager;
        private readonly ITenantCache _tenantCache;
        private readonly AbpLoginResultTypeHelper _abpLoginResultTypeHelper;
        private readonly TokenAuthConfiguration _configuration;
        private readonly IUserManager _userManager;
        private readonly ICacheManager _cacheManager;
        private readonly IOptions<JwtBearerOptions> _jwtOptions;
        private readonly IExternalAuthConfiguration _externalAuthConfiguration;
        private readonly IExternalAuthManager _externalAuthManager;
        private readonly UserRegistrationManager _userRegistrationManager;
        private readonly IImpersonationManager _impersonationManager;
        private readonly IUserLinkManager _userLinkManager;
        private readonly IAppNotifier _appNotifier;
        private readonly ISmsSender _smsSender;
        private readonly IEmailSender _emailSender;
        private readonly IdentityOptions _identityOptions;
        private readonly GoogleAuthenticatorProvider _googleAuthenticatorProvider;
        private readonly IRepository<Tenant> _tenantRepository;
        private readonly IAccountAppService _accountAppService;
        private readonly IRepository<User, long> _userRepository;


        public TokenAuthController(
            LogInManager logInManager,
            ITenantCache tenantCache,
            AbpLoginResultTypeHelper abpLoginResultTypeHelper,
            TokenAuthConfiguration configuration,
            IUserManager userManager,
            ICacheManager cacheManager,
            IOptions<JwtBearerOptions> jwtOptions,
            IExternalAuthConfiguration externalAuthConfiguration,
            IExternalAuthManager externalAuthManager,
            UserRegistrationManager userRegistrationManager,
            IImpersonationManager impersonationManager,
            IUserLinkManager userLinkManager,
            IAppNotifier appNotifier,
            ISmsSender smsSender,
            IEmailSender emailSender,
            IOptions<IdentityOptions> identityOptions,
            GoogleAuthenticatorProvider googleAuthenticatorProvider,
            IRepository<Tenant> tenantRepository,
            IAccountAppService accountAppService,
            IRepository<User, long> userRepository)
        {
            _logInManager = logInManager;
            _tenantCache = tenantCache;
            _abpLoginResultTypeHelper = abpLoginResultTypeHelper;
            _configuration = configuration;
            _userManager = userManager;
            _cacheManager = cacheManager;
            _jwtOptions = jwtOptions;
            _externalAuthConfiguration = externalAuthConfiguration;
            _externalAuthManager = externalAuthManager;
            _userRegistrationManager = userRegistrationManager;
            _impersonationManager = impersonationManager;
            _userLinkManager = userLinkManager;
            _appNotifier = appNotifier;
            _smsSender = smsSender;
            _emailSender = emailSender;
            _googleAuthenticatorProvider = googleAuthenticatorProvider;
            _identityOptions = identityOptions.Value;
            _tenantRepository = tenantRepository;
            _accountAppService = accountAppService;
            _userRepository = userRepository;
        }

        [HttpPost]
        public async Task<AuthenticateResultModel> Authenticate([FromBody] AuthenticateModel model)
        {
            //string hostOrigin; 
            try
            {

                var userForVerification = await _userRepository.FirstOrDefaultAsync(item => item.EmailAddress == model.UserNameOrEmailAddress);
                if (userForVerification == null)
                {
                    throw new UserFriendlyException("No such user");
                }
                if (userForVerification.IsEmailConfirmed == false)
                {
                    throw new Exception("Please confirm registration via email.");
                }
                var loginResult = await GetLoginResultAsync(
                    model.UserNameOrEmailAddress,
                    model.Password,
                    GetTenancyNameOrNull()
                );
                var returnUrl = model.ReturnUrl;

                if (model.SingleSignIn.HasValue && model.SingleSignIn.Value && loginResult.Result == AbpLoginResultType.Success)
                {
                    loginResult.User.SetSignInToken();
                    returnUrl = AddSingleSignInParametersToReturnUrl(model.ReturnUrl, loginResult.User.SignInToken, loginResult.User.Id, loginResult.User.TenantId);
                }

                //Password reset
                if (loginResult.User.ShouldChangePasswordOnNextLogin)
                {
                    loginResult.User.SetNewPasswordResetCode();
                    return new AuthenticateResultModel
                    {
                        ShouldResetPassword = true,
                        PasswordResetCode = loginResult.User.PasswordResetCode,
                        UserId = loginResult.User.Id,
                        ReturnUrl = returnUrl
                    };
                }

                //Two factor auth
                await _userManager.InitializeOptionsAsync(loginResult.Tenant?.Id);
                string twoFactorRememberClientToken = null;
                if (await IsTwoFactorAuthRequiredAsync(loginResult, model))
                {
                    if (model.TwoFactorVerificationCode.IsNullOrEmpty())
                    {
                        //Add a cache item which will be checked in SendTwoFactorAuthCode to prevent sending unwanted two factor code to users.
                        _cacheManager
                            .GetTwoFactorCodeCache()
                            .Set(
                                loginResult.User.ToUserIdentifier().ToString(),
                                new TwoFactorCodeCacheItem()
                            );

                        return new AuthenticateResultModel
                        {
                            RequiresTwoFactorVerification = true,
                            UserId = loginResult.User.Id,
                            TwoFactorAuthProviders = await _userManager.GetValidTwoFactorProvidersAsync(loginResult.User),
                            ReturnUrl = returnUrl
                        };
                    }

                    twoFactorRememberClientToken = await TwoFactorAuthenticateAsync(loginResult.User, model);
                }

                //Login!
                var accessToken = CreateAccessToken(CreateJwtClaims(loginResult.Identity));
                var user = _userRepository.Get(loginResult.User.Id);
                return new AuthenticateResultModel
                {
                    AccessToken = accessToken,
                    EncryptedAccessToken = GetEncrpyedAccessToken(accessToken),
                    ExpireInSeconds = (int)_configuration.Expiration.TotalSeconds,
                    TwoFactorRememberClientToken = twoFactorRememberClientToken,
                    ReturnUrl = returnUrl,
                    AmountTokens = user.AmountTokens
                };
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
         
        [HttpGet]
        public async Task<TenantModel> GetTenant()
        {
           return await GetTenantFromRequest(Request);
        }

        [HttpPost]
        public async Task SendTwoFactorAuthCode([FromBody] SendTwoFactorAuthCodeModel model)
        {
            var cacheKey = new UserIdentifier(AbpSession.TenantId, model.UserId).ToString();

            var cacheItem = await _cacheManager
                .GetTwoFactorCodeCache()
                .GetOrDefaultAsync(cacheKey);

            if (cacheItem == null)
            {
                //There should be a cache item added in Authenticate method! This check is needed to prevent sending unwanted two factor code to users.
                throw new UserFriendlyException(L("SendSecurityCodeErrorMessage"));
            }

            var user = await _userManager.FindByIdAsync(model.UserId.ToString());

            if (model.Provider != GoogleAuthenticatorProvider.Name)
            {
                cacheItem.Code = await _userManager.GenerateTwoFactorTokenAsync(user, model.Provider);
                var message = L("EmailSecurityCodeBody", cacheItem.Code);

                if (model.Provider == "Email")
                {
                    await _emailSender.SendAsync(await _userManager.GetEmailAsync(user), L("EmailSecurityCodeSubject"),
                        message);
                }
                else if (model.Provider == "Phone")
                {
                    await _smsSender.SendAsync(await _userManager.GetPhoneNumberAsync(user), message);
                }
            }

            _cacheManager.GetTwoFactorCodeCache().Set(
                    cacheKey,
                    cacheItem
                );
            _cacheManager.GetCache("ProviderCache").Set(
                "Provider",
                model.Provider
            );
        }

        [HttpPost]
        public async Task<ImpersonatedAuthenticateResultModel> ImpersonatedAuthenticate(string impersonationToken)
        {
            var result = await _impersonationManager.GetImpersonatedUserAndIdentity(impersonationToken);
            var accessToken = CreateAccessToken(CreateJwtClaims(result.Identity));

            return new ImpersonatedAuthenticateResultModel
            {
                AccessToken = accessToken,
                EncryptedAccessToken = GetEncrpyedAccessToken(accessToken),
                ExpireInSeconds = (int)_configuration.Expiration.TotalSeconds
            };
        }

        [HttpPost]
        public async Task<SwitchedAccountAuthenticateResultModel> LinkedAccountAuthenticate(string switchAccountToken)
        {
            var result = await _userLinkManager.GetSwitchedUserAndIdentity(switchAccountToken);
            var accessToken = CreateAccessToken(CreateJwtClaims(result.Identity));

            return new SwitchedAccountAuthenticateResultModel
            {
                AccessToken = accessToken,
                EncryptedAccessToken = GetEncrpyedAccessToken(accessToken),
                ExpireInSeconds = (int)_configuration.Expiration.TotalSeconds
            };
        }

        [HttpGet]
        public List<ExternalLoginProviderInfoModel> GetExternalAuthenticationProviders()
        {
            return ObjectMapper.Map<List<ExternalLoginProviderInfoModel>>(_externalAuthConfiguration.Providers);
        }

        [HttpPost]
        public async Task<ExternalAuthenticateResultModel> ExternalAuthenticate([FromBody] ExternalAuthenticateModel model)
        {
            var externalUser = await GetExternalUserInfo(model);

            var loginResult = await _logInManager.LoginAsync(new UserLoginInfo(model.AuthProvider, model.ProviderKey, model.AuthProvider), GetTenancyNameOrNull());

            switch (loginResult.Result)
            {
                case AbpLoginResultType.Success:
                    {
                        var accessToken = CreateAccessToken(CreateJwtClaims(loginResult.Identity));

                        var returnUrl = model.ReturnUrl;

                        if (model.SingleSignIn.HasValue && model.SingleSignIn.Value && loginResult.Result == AbpLoginResultType.Success)
                        {
                            loginResult.User.SetSignInToken();
                            returnUrl = AddSingleSignInParametersToReturnUrl(model.ReturnUrl, loginResult.User.SignInToken, loginResult.User.Id, loginResult.User.TenantId);
                        }

                        return new ExternalAuthenticateResultModel
                        {
                            AccessToken = accessToken,
                            EncryptedAccessToken = GetEncrpyedAccessToken(accessToken),
                            ExpireInSeconds = (int)_configuration.Expiration.TotalSeconds,
                            ReturnUrl = returnUrl
                        };
                    }
                case AbpLoginResultType.UnknownExternalLogin:
                    {
                        var newUser = await RegisterExternalUserAsync(externalUser);
                        if (!newUser.IsActive)
                        {
                            return new ExternalAuthenticateResultModel
                            {
                                WaitingForActivation = true
                            };
                        }

                        //Try to login again with newly registered user!
                        loginResult = await _logInManager.LoginAsync(new UserLoginInfo(model.AuthProvider, model.ProviderKey, model.AuthProvider), GetTenancyNameOrNull());
                        if (loginResult.Result != AbpLoginResultType.Success)
                        {
                            throw _abpLoginResultTypeHelper.CreateExceptionForFailedLoginAttempt(
                                loginResult.Result,
                                model.ProviderKey,
                                GetTenancyNameOrNull()
                            );
                        }

                        var accessToken = CreateAccessToken(CreateJwtClaims(loginResult.Identity));
                        return new ExternalAuthenticateResultModel
                        {
                            AccessToken = accessToken,
                            EncryptedAccessToken = GetEncrpyedAccessToken(accessToken),
                            ExpireInSeconds = (int)_configuration.Expiration.TotalSeconds
                        };
                    }
                default:
                    {
                        throw _abpLoginResultTypeHelper.CreateExceptionForFailedLoginAttempt(
                            loginResult.Result,
                            model.ProviderKey,
                            GetTenancyNameOrNull()
                        );
                    }
            }
        }

        private async Task<TenantModel> GetTenantFromRequest(Microsoft.AspNetCore.Http.HttpRequest request)
        {

            request.Headers.TryGetValue("Origin", out var urlOrigin);
            var urlOriginString = urlOrigin.ToString();
            //long? tenantId = null;
            var tenantDto = new TenantModel();
            if (!urlOriginString.IsNullOrEmpty())
            {
                var tenant = await _tenantRepository.FirstOrDefaultAsync(x => x.TenantUrl == urlOriginString);
                if (tenant != null)
                {
                    tenantDto.TenantId = tenant.Id;
                    tenantDto.LogoUrl = tenant.LogoUrl;
                }
            }
            return tenantDto;
        }

        #region Etc

        [AbpMvcAuthorize]
        [HttpGet]
        public async Task<ActionResult> TestNotification(string message = "", string severity = "info")
        {
            if (message.IsNullOrEmpty())
            {
                message = "This is a test notification, created at " + Clock.Now;
            }

            await _appNotifier.SendMessageAsync(
                AbpSession.ToUserIdentifier(),
                message,
                severity.ToPascalCase().ToEnum<NotificationSeverity>()
                );

            return Content("Sent notification: " + message);
        }

        #endregion

        private async Task<User> RegisterExternalUserAsync(ExternalAuthUserInfo externalLoginInfo)
        {
            var user = await _userRegistrationManager.RegisterAsync(

                externalLoginInfo.FirstName,
                externalLoginInfo.LastName,
                externalLoginInfo.UserName,
                externalLoginInfo.YourName,
                externalLoginInfo.ContentType,
                externalLoginInfo.ContentVolume,
                externalLoginInfo.Industry,
                externalLoginInfo.IsContentShedule,
                externalLoginInfo.CMS,
                externalLoginInfo.Website,
                externalLoginInfo.Company,
                externalLoginInfo.UserType,
                externalLoginInfo.PhoneNumber,
                externalLoginInfo.EmailAddress,
                externalLoginInfo.Password,
                true
            );


            user.Logins = new List<UserLogin>
            {
                new UserLogin
                {
                    LoginProvider = externalLoginInfo.Provider,
                    ProviderKey = externalLoginInfo.ProviderKey,
                    TenantId = user.TenantId
                }
            };

            await CurrentUnitOfWork.SaveChangesAsync();

            return user;
        }

        private async Task<ExternalAuthUserInfo> GetExternalUserInfo(ExternalAuthenticateModel model)
        {
            var userInfo = await _externalAuthManager.GetUserInfo(model.AuthProvider, model.ProviderAccessCode);
            if (userInfo.ProviderKey != model.ProviderKey)
            {
                throw new UserFriendlyException(L("CouldNotValidateExternalUser"));
            }

            return userInfo;
        }

        private async Task<bool> IsTwoFactorAuthRequiredAsync(AbpLoginResult<Tenant, User> loginResult, AuthenticateModel authenticateModel)
        {
            if (!await SettingManager.GetSettingValueAsync<bool>(AbpZeroSettingNames.UserManagement.TwoFactorLogin.IsEnabled))
            {
                return false;
            }

            if (!loginResult.User.IsTwoFactorEnabled)
            {
                return false;
            }

            if ((await _userManager.GetValidTwoFactorProvidersAsync(loginResult.User)).Count <= 0)
            {
                return false;
            }

            if (await TwoFactorClientRememberedAsync(loginResult.User.ToUserIdentifier(), authenticateModel))
            {
                return false;
            }

            return true;
        }

        private async Task<bool> TwoFactorClientRememberedAsync(UserIdentifier userIdentifier, AuthenticateModel authenticateModel)
        {
            if (!await SettingManager.GetSettingValueAsync<bool>(AbpZeroSettingNames.UserManagement.TwoFactorLogin.IsRememberBrowserEnabled))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(authenticateModel.TwoFactorRememberClientToken))
            {
                return false;
            }

            try
            {
                var validationParameters = new TokenValidationParameters
                {
                    ValidAudience = _configuration.Audience,
                    ValidIssuer = _configuration.Issuer,
                    IssuerSigningKey = _configuration.SecurityKey
                };

                foreach (var validator in _jwtOptions.Value.SecurityTokenValidators)
                {
                    if (validator.CanReadToken(authenticateModel.TwoFactorRememberClientToken))
                    {
                        try
                        {
                            SecurityToken validatedToken;
                            var principal = validator.ValidateToken(authenticateModel.TwoFactorRememberClientToken, validationParameters, out validatedToken);
                            var useridentifierClaim = principal.FindFirst(c => c.Type == UserIdentifierClaimType);
                            if (useridentifierClaim == null)
                            {
                                return false;
                            }

                            return useridentifierClaim.Value == userIdentifier.ToString();
                        }
                        catch (Exception ex)
                        {
                            Logger.Debug(ex.ToString(), ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Debug(ex.ToString(), ex);
            }

            return false;
        }

        /* Checkes two factor code and returns a token to remember the client (browser) if needed */
        private async Task<string> TwoFactorAuthenticateAsync(User user, AuthenticateModel authenticateModel)
        {
            var twoFactorCodeCache = _cacheManager.GetTwoFactorCodeCache();
            var userIdentifier = user.ToUserIdentifier().ToString();
            var cachedCode = await twoFactorCodeCache.GetOrDefaultAsync(userIdentifier);
            var provider = _cacheManager.GetCache("ProviderCache").Get("Provider", cache => cache).ToString();

            if (provider == GoogleAuthenticatorProvider.Name)
            {
                await _googleAuthenticatorProvider.ValidateAsync("TwoFactor", authenticateModel.TwoFactorVerificationCode, _userManager, user);
            }
            else if (cachedCode?.Code == null || cachedCode.Code != authenticateModel.TwoFactorVerificationCode)
            {
                throw new UserFriendlyException(L("InvalidSecurityCode"));
            }

            //Delete from the cache since it was a single usage code
            await twoFactorCodeCache.RemoveAsync(userIdentifier);

            if (authenticateModel.RememberClient)
            {
                if (await SettingManager.GetSettingValueAsync<bool>(AbpZeroSettingNames.UserManagement.TwoFactorLogin.IsRememberBrowserEnabled))
                {
                    return CreateAccessToken(new[]
                        {
                            new Claim(UserIdentifierClaimType, user.ToUserIdentifier().ToString())
                        },
                        TimeSpan.FromDays(365)
                    );
                }
            }

            return null;
        }

        private string GetTenancyNameOrNull()
        {
            if (!AbpSession.TenantId.HasValue)
            {
                return null;
            }

            return _tenantRepository.Get((int)AbpSession.TenantId)?.TenancyName;
        }

        private async Task<AbpLoginResult<Tenant, User>> GetLoginResultAsync(string usernameOrEmailAddress, string password, string tenancyName)
        {
            var loginResult = await _logInManager.LoginAsync(usernameOrEmailAddress, password, tenancyName);
            
            switch (loginResult.Result)
            {
                case AbpLoginResultType.Success:
                    return loginResult; 
                default: 
                    throw _abpLoginResultTypeHelper.CreateExceptionForFailedLoginAttempt(loginResult.Result, usernameOrEmailAddress, tenancyName);
            }
        }

        private string CreateAccessToken(IEnumerable<Claim> claims, TimeSpan? expiration = null)
        {
            var now = DateTime.UtcNow;

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _configuration.Issuer,
                audience: _configuration.Audience,
                claims: claims,
                notBefore: now,
                expires: now.Add(expiration ?? _configuration.Expiration),
                signingCredentials: _configuration.SigningCredentials
            );

            return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        }

        private string GetEncrpyedAccessToken(string accessToken)
        {
            return SimpleStringCipher.Instance.Encrypt(accessToken, AppConsts.DefaultPassPhrase);
        }

        private List<Claim> CreateJwtClaims(ClaimsIdentity identity)
        {
            var claims = identity.Claims.ToList();
            var nameIdClaim = claims.First(c => c.Type == _identityOptions.ClaimsIdentity.UserIdClaimType);

            if (_identityOptions.ClaimsIdentity.UserIdClaimType != JwtRegisteredClaimNames.Sub)
            {
                claims.Add(new Claim(JwtRegisteredClaimNames.Sub, nameIdClaim.Value));
            }

            claims.AddRange(new[]
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.Now.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            });

            return claims;
        }

        private string AddSingleSignInParametersToReturnUrl(string returnUrl, string signInToken, long userId, int? tenantId)
        {
            returnUrl += (returnUrl.Contains("?") ? "&" : "?") +
                         "accessToken=" + signInToken +
                         "&userId=" + Convert.ToBase64String(Encoding.UTF8.GetBytes(userId.ToString()));
            if (tenantId.HasValue)
            {
                returnUrl += "&tenantId=" + Convert.ToBase64String(Encoding.UTF8.GetBytes(tenantId.Value.ToString()));
            }

            return returnUrl;
        }
    }
}
