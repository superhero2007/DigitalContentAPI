using System;
using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Configuration;
using Abp.Extensions;
using Abp.Runtime.Session;
using Abp.UI;
using Abp.Zero.Configuration;
using Microsoft.AspNetCore.Identity;
using Digital_Content.DigitalContent.Authorization.Accounts.Dto;
using Digital_Content.DigitalContent.Authorization.Impersonation;
using Digital_Content.DigitalContent.Authorization.Users;
using Digital_Content.DigitalContent.Configuration;
using Digital_Content.DigitalContent.Debugging;
using Digital_Content.DigitalContent.MultiTenancy;
using Digital_Content.DigitalContent.Security.Recaptcha;
using Digital_Content.DigitalContent.Url;
using Digital_Content.DigitalContent.MultiTenancy.Dto;

namespace Digital_Content.DigitalContent.Authorization.Accounts
{
    public class AccountAppService : DigitalContentAppServiceBase, IAccountAppService
    {
        public IAppUrlService AppUrlService { get; set; }

        public IRecaptchaValidator RecaptchaValidator { get; set; }

        private readonly IUserEmailer _userEmailer;
        private readonly UserRegistrationManager _userRegistrationManager;
        private readonly IImpersonationManager _impersonationManager;
        private readonly IUserLinkManager _userLinkManager;
        private readonly IPasswordHasher<User> _passwordHasher;
        
        private readonly ITenantAppService _tenantService;


        public AccountAppService(
            IUserEmailer userEmailer, 
            UserRegistrationManager userRegistrationManager, 
            IImpersonationManager impersonationManager, 
            IUserLinkManager userLinkManager,
            IPasswordHasher<User> passwordHasher,
            ITenantAppService tenantService)
        {
            _userEmailer = userEmailer;
            _userRegistrationManager = userRegistrationManager;
            _impersonationManager = impersonationManager;
            _userLinkManager = userLinkManager;
            _passwordHasher = passwordHasher;
            _tenantService = tenantService;

            AppUrlService = NullAppUrlService.Instance;
            RecaptchaValidator = NullRecaptchaValidator.Instance;
        }

        public async Task<IsTenantAvailableOutput> IsTenantAvailable(IsTenantAvailableInput input)
        {
            
            var tenant = await TenantManager.FindByTenancyNameAsync(input.TenancyName);
            if (tenant == null)
            {
                return new IsTenantAvailableOutput(TenantAvailabilityState.NotFound);
            }

            if (!tenant.IsActive)
            {
                return new IsTenantAvailableOutput(TenantAvailabilityState.InActive);
            }

            return new IsTenantAvailableOutput(TenantAvailabilityState.Available, tenant.Id);
        }

        public async Task<RegisterOutput> Register(RegisterInput input)
        {

            try
            {
                var user = await _userRegistrationManager.RegisterAsync(
               input.FirstName,
               input.LastName,
               input.UserName,
               input.YourName,
               input.ContentType,
               input.ContentVolume,
               input.Industry,
               input.IsContentShedule,
               input.CMS,
               input.Website,
               input.Company,
               input.UserType,
               input.PhoneNumber,
               input.EmailAddress,
               input.Password,
               false
           );

                var isEmailConfirmationRequiredForLogin = await SettingManager.GetSettingValueAsync<bool>(AbpZeroSettingNames.UserManagement.IsEmailConfirmationRequiredForLogin);
                await SendEmailActivationLink(new SendEmailActivationLinkInput()
                {
                    EmailAddress = input.EmailAddress
                });
                return new RegisterOutput
                {
                    CanLogin = user.IsActive && (user.IsEmailConfirmed || !isEmailConfirmationRequiredForLogin)
                };
            }
            catch (Exception ex)
             {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public async Task SendPasswordResetCode(SendPasswordResetCodeInput input)
        {
            var user = await GetUserByChecking(input.EmailAddress);
            user.SetNewPasswordResetCode();
            await _userEmailer.SendPasswordResetLinkAsync(
                user,
                AppUrlService.CreatePasswordResetUrlFormat(AbpSession.TenantId)
                );
        }

        public async Task<ResetPasswordOutput> ResetPassword(ResetPasswordInput input)
        {
            var user = await UserManager.GetUserByIdAsync(input.UserId);
            if (user == null) // || user.PasswordResetCode.IsNullOrEmpty() || user.PasswordResetCode != input.ResetCode)
            {
                throw new UserFriendlyException(L("InvalidPasswordResetCode"), L("InvalidPasswordResetCode_Detail"));
            }

            user.Password = _passwordHasher.HashPassword(user, input.Password);
            user.PasswordResetCode = null;
            user.IsEmailConfirmed = true;
            user.ShouldChangePasswordOnNextLogin = false;

            await UserManager.UpdateAsync(user);

            return new ResetPasswordOutput
            {
                CanLogin = user.IsActive,
                UserName = user.UserName
            };
        }

        public async Task SendEmailActivationLink(SendEmailActivationLinkInput input)
        {
            var user = await GetUserByChecking(input.EmailAddress);
            user.SetNewEmailConfirmationCode();
            await _userEmailer.SendEmailActivationLinkAsync(
                user,
                AppUrlService.CreateEmailActivationUrlFormat(AbpSession.TenantId)
            );
        }

        public async Task ActivateEmail(ActivateEmailInput input)
        {
            var user = await UserManager.GetUserByIdAsync(input.UserId);
            if (user == null || user.EmailConfirmationCode.IsNullOrEmpty() || user.EmailConfirmationCode != input.ConfirmationCode)
            {
                throw new UserFriendlyException(L("InvalidEmailConfirmationCode"), L("InvalidEmailConfirmationCode_Detail"));
            }

            user.IsEmailConfirmed = true;
            user.EmailConfirmationCode = null;
            user.IsActive = true;
            await UserManager.UpdateAsync(user);
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_Users_Impersonation)]
        public virtual async Task<ImpersonateOutput> Impersonate(ImpersonateInput input)
        {
            return new ImpersonateOutput
            {
                ImpersonationToken = await _impersonationManager.GetImpoersonateToken(input.UserId, input.TenantId),
                TenancyName = await GetTenancyNameOrNullAsync(input.TenantId)
            };
        }

        public virtual async Task<ImpersonateOutput> BackToImpersonator()
        {
            return new ImpersonateOutput
            {
                ImpersonationToken = await _impersonationManager.GetBackToImpersonatorToken(),
                TenancyName = await GetTenancyNameOrNullAsync(AbpSession.ImpersonatorTenantId)
            };
        }

        public virtual async Task<SwitchToLinkedAccountOutput> SwitchToLinkedAccount(SwitchToLinkedAccountInput input)
        {
            if (!await _userLinkManager.AreUsersLinked(AbpSession.ToUserIdentifier(), input.ToUserIdentifier()))
            {
                throw new Exception(L("This account is not linked to your account"));
            }

            return new SwitchToLinkedAccountOutput
            {
                SwitchAccountToken = await _userLinkManager.GetAccountSwitchToken(input.TargetUserId, input.TargetTenantId),
                TenancyName = await GetTenancyNameOrNullAsync(input.TargetTenantId)
            };
        }

        private bool UseCaptchaOnRegistration()
        {
            if (DebugHelper.IsDebug)
            {
                return false;
            }
            
            return SettingManager.GetSettingValue<bool>(AppSettings.UserManagement.UseCaptchaOnRegistration);
        }

        private async Task<Tenant> GetActiveTenantAsync(int tenantId)
        {
            var tenant = await TenantManager.FindByIdAsync(tenantId);
            if (tenant == null)
            {
                throw new UserFriendlyException(L("UnknownTenantId{0}", tenantId));
            }

            if (!tenant.IsActive)
            {
                throw new UserFriendlyException(L("TenantIdIsNotActive{0}", tenantId));
            }

            return tenant;
        }
        
        private async Task<string> GetTenancyNameOrNullAsync(int? tenantId)
        {
            return tenantId.HasValue ? (await GetActiveTenantAsync(tenantId.Value)).TenancyName : null;
        }

        private async Task<User> GetUserByChecking(string inputEmailAddress)
        {
            var user = await UserManager.FindByEmailAsync(inputEmailAddress);
            if (user == null)
            {
                throw new UserFriendlyException(L("InvalidEmailAddress"));
            }

            return user;
        }
    }
}