using System.Reflection;
using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Digital_Content.DigitalContent.Authorization;
using Digital_Content.DigitalContent.Authorization.Companies;
using Abp.Dependency;

namespace Digital_Content.DigitalContent
{
    /// <summary>
    /// Application layer module of the application.
    /// </summary>
    [DependsOn(
        typeof(DigitalContentCoreModule)
        )]
    public class DigitalContentApplicationModule : AbpModule
    {
        public override void PreInitialize()
        {
            //Adding authorization providers
            Configuration.Authorization.Providers.Add<AppAuthorizationProvider>();

            //Adding custom AutoMapper configuration
            Configuration.Modules.AbpAutoMapper().Configurators.Add(CustomDtoMapper.CreateMappings);

        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(DigitalContentApplicationModule).GetAssembly());
            
        }
    }
}