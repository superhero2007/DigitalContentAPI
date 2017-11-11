using Abp.Authorization;
using Abp.Configuration;
using Abp.Domain.Uow;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Digital_Content.DigitalContent.Authorization.Roles;
using Digital_Content.DigitalContent.Authorization.Users;
using Digital_Content.DigitalContent.MultiTenancy;

namespace Digital_Content.DigitalContent.Identity
{
    public class SignInManager : AbpSignInManager<Tenant, Role, User>
    {
        public SignInManager(
            IUserManager userManager, 
            IHttpContextAccessor contextAccessor,
            UserClaimsPrincipalFactory claimsFactory, 
            IOptions<IdentityOptions> optionsAccessor, 
            ILogger<SignInManager<User>> logger,
            IUnitOfWorkManager unitOfWorkManager,
            ISettingManager settingManager
            ) : base(
                userManager, 
                contextAccessor, 
                claimsFactory, 
                optionsAccessor, 
                logger,
                unitOfWorkManager,
                settingManager)
        {
        }
    }
}
