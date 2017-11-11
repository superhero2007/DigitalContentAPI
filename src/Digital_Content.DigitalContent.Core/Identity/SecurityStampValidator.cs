using Abp.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;
using Digital_Content.DigitalContent.Authorization.Roles;
using Digital_Content.DigitalContent.Authorization.Users;
using Digital_Content.DigitalContent.MultiTenancy;

namespace Digital_Content.DigitalContent.Identity
{
    public class SecurityStampValidator : AbpSecurityStampValidator<Tenant, Role, User>
    {
        public SecurityStampValidator(
            IOptions<IdentityOptions> options, 
            SignInManager signInManager) 
            : base(options, signInManager)
        {
        }
    }
}