using System.Collections.Generic;

namespace Digital_Content.DigitalContent.Web.Authentication.External
{
    public interface IExternalAuthConfiguration
    {
        List<ExternalLoginProviderInfo> Providers { get; }
    }
}