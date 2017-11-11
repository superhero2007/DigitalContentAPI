using System.Collections.Generic;
using Digital_Content.DigitalContent.Url;

namespace Digital_Content.DigitalContent.Tests.Url
{
    public class FakeWebUrlService : IWebUrlService
    {
        public string WebSiteRootAddressFormat { get; }

        public string ServerRootAddressFormat { get; }

        public bool SupportsTenancyNameInUrl { get; }

        public string GetSiteRootAddress(string tenancyName = null)
        {
            return "http://test.com/";
        }

        public string GetServerRootAddress(string tenancyName = null)
        {
            return "http://test.com/";
        }

        public List<string> GetRedirectAllowedExternalWebSites()
        {
            return new List<string>();
        }
    }
}