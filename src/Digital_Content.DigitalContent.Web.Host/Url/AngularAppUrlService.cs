using Abp.Domain.Repositories;
using Abp.MultiTenancy;
using Digital_Content.DigitalContent.MultiTenancy;
using Digital_Content.DigitalContent.Url;

namespace Digital_Content.DigitalContent.Web.Url
{
    public class AngularAppUrlService : AppUrlServiceBase
    {
        public override string EmailActivationRoute => "account/confirm-email";

        public override string PasswordResetRoute => "account/reset-password";

        public AngularAppUrlService(
                IWebUrlService webUrlService,
                ITenantCache tenantCache,
                IRepository<Tenant, int> tenantRepository
            ) : base(
                webUrlService,
                tenantCache,
                tenantRepository
            )
        {

        }
    }
}