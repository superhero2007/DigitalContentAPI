using System.Text;
using Abp.Dependency;
using Abp.Extensions;
using Abp.IO.Extensions;
using Abp.Reflection.Extensions;
using Digital_Content.DigitalContent.Url;
using Abp.Domain.Repositories;
using Digital_Content.DigitalContent.MultiTenancy;

namespace Digital_Content.DigitalContent.Emailing
{
    public class EmailTemplateProvider : IEmailTemplateProvider, ITransientDependency
    {
        private readonly IWebUrlService _webUrlService;
        private readonly IRepository<Tenant> _tenantRepository;

        public EmailTemplateProvider(
            IWebUrlService webUrlService,
            IRepository<Tenant> tenantRepository)
        {
            _webUrlService = webUrlService;
            _tenantRepository = tenantRepository;
        }

        public string GetDefaultTemplate(int? tenantId)
        {
            using (var stream = typeof(EmailTemplateProvider).GetAssembly().GetManifestResourceStream("Digital_Content.DigitalContent.Emailing.EmailTemplates.default.html"))
            {
                var bytes = stream.GetAllBytes();
                var template = Encoding.UTF8.GetString(bytes, 3, bytes.Length - 3);
                return template.Replace("{EMAIL_LOGO_URL}", GetTenantLogoUrl(tenantId));
            }
        }

        private string GetTenantLogoUrl(int? tenantId)
        {

            if (!tenantId.HasValue)
            {
                return string.Empty;
                    //_webUrlService.GetServerRootAddress().EnsureEndsWith('/') + "TenantCustomization/GetTenantLogo";
            }
            var tenant = _tenantRepository.Get((int)tenantId);
            return tenant.LogoUrl;
            //_webUrlService.GetServerRootAddress().EnsureEndsWith('/') + "TenantCustomization/GetTenantLogo?tenantId=" + tenantId.Value;
        }
    }
}