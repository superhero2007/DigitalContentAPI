using Microsoft.Extensions.Configuration;

namespace Digital_Content.DigitalContent.Configuration
{
    public interface IAppConfigurationAccessor
    {
        IConfigurationRoot Configuration { get; }
    }
}
