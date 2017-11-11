using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Digital_Content.DigitalContent.Authentication.TwoFactor.Google;
using Digital_Content.DigitalContent.Authorization.Roles;
using Digital_Content.DigitalContent.Authorization.Users;
using Digital_Content.DigitalContent.MultiTenancy;

namespace Digital_Content.DigitalContent.Identity
{
    public static class IdentityRegistrar
    {
        private const string CookiePrefix = "Identity.DigitalContent";

        public static void Register(IServiceCollection services, string cookiePostFix)
        {
            services.AddLogging();

            services.AddAbpIdentity<Tenant, User, Role>(options =>
                {
                    options.Cookies.ApplicationCookie.CookieName = CookiePrefix + "." + cookiePostFix;

                    options.Cookies.ExternalCookie.CookieName = CookiePrefix + ".External." + cookiePostFix;
                    options.Cookies.ExternalCookie.CookieName = CookiePrefix + ".TwoFactorRememberMe." + cookiePostFix;
                    options.Cookies.ExternalCookie.CookieName = CookiePrefix + ".TwoFactorUserId." + cookiePostFix;

                    options.Tokens.ProviderMap[GoogleAuthenticatorProvider.Name] = new TokenProviderDescriptor(typeof(GoogleAuthenticatorProvider));
                })
                .AddAbpSecurityStampValidator<SecurityStampValidator>()
                .AddAbpUserManager<IUserManager>()
                .AddAbpRoleManager<RoleManager>()
                .AddAbpSignInManager<SignInManager>()
                .AddAbpUserClaimsPrincipalFactory<UserClaimsPrincipalFactory>()
                .AddDefaultTokenProviders();
        }
    }
}
