using Abp.Events.Bus;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Castle.MicroKernel.Registration;
using Microsoft.Extensions.Configuration;
using Digital_Content.DigitalContent.Configuration;
using Digital_Content.DigitalContent.EntityFrameworkCore;
using Digital_Content.DigitalContent.Migrator.DependencyInjection;

namespace Digital_Content.DigitalContent.Migrator
{
    [DependsOn(typeof(DigitalContentEntityFrameworkCoreModule))]
    public class DigitalContentMigratorModule : AbpModule
    {
        private readonly IConfigurationRoot _appConfiguration;

        public DigitalContentMigratorModule(DigitalContentEntityFrameworkCoreModule abpZeroTemplateEntityFrameworkCoreModule)
        {
            abpZeroTemplateEntityFrameworkCoreModule.SkipDbSeed = true;

            _appConfiguration = AppConfigurations.Get(
                typeof(DigitalContentMigratorModule).GetAssembly().GetDirectoryPathOrNull()
            );
        }

        public override void PreInitialize()
        {
            Configuration.DefaultNameOrConnectionString = _appConfiguration.GetConnectionString(
                DigitalContentConsts.ConnectionStringName
                );

            Configuration.BackgroundJobs.IsJobExecutionEnabled = false;
            Configuration.ReplaceService(typeof(IEventBus), () =>
            {
                IocManager.IocContainer.Register(
                    Component.For<IEventBus>().Instance(NullEventBus.Instance)
                );
            });
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(DigitalContentMigratorModule).GetAssembly());
            ServiceCollectionRegistrar.Register(IocManager);
        }
    }
}