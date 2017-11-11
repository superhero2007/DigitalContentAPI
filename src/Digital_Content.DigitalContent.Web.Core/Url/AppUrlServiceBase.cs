using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.MultiTenancy;
using Digital_Content.DigitalContent.MultiTenancy;
using Digital_Content.DigitalContent.Url;

namespace Digital_Content.DigitalContent.Web.Url
{
    public abstract class AppUrlServiceBase : IAppUrlService, ITransientDependency
    {
        public abstract string EmailActivationRoute { get; }

        public abstract string PasswordResetRoute { get; }

        protected readonly IWebUrlService WebUrlService;
        protected readonly ITenantCache TenantCache;
        private readonly IRepository<Tenant, int> _tenantRepository;

        protected AppUrlServiceBase(IWebUrlService webUrlService, ITenantCache tenantCache, IRepository<Tenant, int> tenantRepository)
        {
            WebUrlService = webUrlService;
            TenantCache = tenantCache;
            _tenantRepository = tenantRepository;
        }

        public string CreateEmailActivationUrlFormat(int? tenantId)
        {
            if (tenantId.HasValue)
            {
                var rootAddress = _tenantRepository.Get((int)tenantId).TenantUrl;
                return CreateEmailActivationUrlFormatFromTenantUrl(rootAddress);
            }
            else
            {
                return CreateEmailActivationUrlFormat(GetTenancyName(tenantId));
            }
        }

        public string CreatePasswordResetUrlFormat(int? tenantId)
        {
            if (tenantId.HasValue)
            {
                var rootAddress = _tenantRepository.Get((int)tenantId).TenantUrl;
                return CreatePasswordResetUrlFormatFromTenantUrl(rootAddress);
            }
            else
            {
                return CreatePasswordResetUrlFormat(GetTenancyName(tenantId));
            }
        }

        public string CreateEmailActivationUrlFormat(string tenancyName)
        {
            var activationLink = WebUrlService.GetSiteRootAddress(tenancyName).EnsureEndsWith('/') + EmailActivationRoute + "?userId={userId}&confirmationCode={confirmationCode}";

            if (tenancyName != null)
            {
                activationLink = activationLink + "&tenantId={tenantId}";
            }

            return activationLink;
        }
        public string CreateEmailActivationUrlFormatFromTenantUrl(string tenantUrl)
        {
            var activationLink = tenantUrl.EnsureEndsWith('/') + EmailActivationRoute + "?userId={userId}&confirmationCode={confirmationCode}";

            if (tenantUrl != null)
            {
                activationLink = activationLink + "&tenantId={tenantId}";
            }

            return activationLink;
        }


        public string CreatePasswordResetUrlFormat(string tenancyName)
        {
            var resetLink = WebUrlService.GetSiteRootAddress(tenancyName).EnsureEndsWith('/') + PasswordResetRoute + "?userId={userId}&resetCode={resetCode}";

            if (tenancyName != null)
            {
                resetLink = resetLink + "&tenantId={tenantId}";
            }

            return resetLink;
        }

        public string CreatePasswordResetUrlFormatFromTenantUrl(string tenantUrl)
        {
            var resetLink = tenantUrl.EnsureEndsWith('/') + PasswordResetRoute + "?userId={userId}&resetCode={resetCode}";

            if (tenantUrl != null)
            {
                resetLink = resetLink + "&tenantId={tenantId}";
            }

            return resetLink;
        }


        private string GetTenancyName(int? tenantId)
        {
            return tenantId.HasValue ? TenantCache.Get(tenantId.Value).TenancyName : null;
        }
    }
}