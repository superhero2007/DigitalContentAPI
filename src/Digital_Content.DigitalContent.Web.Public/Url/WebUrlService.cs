using Abp.Dependency;
using Abp.MultiTenancy;
using Microsoft.AspNetCore.Hosting;
using Digital_Content.DigitalContent.Url;
using Digital_Content.DigitalContent.Web.Url;

namespace Digital_Content.DigitalContent.Web.Public.Url
{
    public class WebUrlService : WebUrlServiceBase, IWebUrlService, ITransientDependency
    {
        public WebUrlService(
            IHostingEnvironment hostingEnvironment,
            ITenantCache tenantCache) :
            base(hostingEnvironment, tenantCache)
        {
        }

        public override string WebSiteRootAddressFormatKey => "App:WebSiteRootAddress";

        public override string ServerRootAddressFormatKey => "App:AdminWebSiteRootAddress";
    }
}