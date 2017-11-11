using Abp.Dependency;
using Abp.Reflection.Extensions;
using Microsoft.Extensions.Configuration;
using Digital_Content.DigitalContent.Configuration;

namespace Digital_Content.DigitalContent.Tests.Configuration
{
    public class TestAppConfigurationAccessor : IAppConfigurationAccessor, ISingletonDependency
    {
        public IConfigurationRoot Configuration { get; }

        public TestAppConfigurationAccessor()
        {
            Configuration = AppConfigurations.Get(
                typeof(DigitalContentTestModule).GetAssembly().GetDirectoryPathOrNull()
            );
        }
    }
}
