using System;
using System.Reflection;
using Abp.AutoMapper;
using Abp.Dependency;
using Abp.Modules;
using Abp.Net.Mail;
using Abp.Reflection.Extensions;
using Abp.Timing;
using Abp.Configuration.Startup;
using Abp.MailKit;
using Abp.Net.Mail.Smtp;
using Abp.Zero;
using Abp.Zero.Configuration;
using Castle.MicroKernel.Registration;
using Digital_Content.DigitalContent.Authorization.Roles;
using Digital_Content.DigitalContent.Authorization.Users;
using Digital_Content.DigitalContent.Chat;
using Digital_Content.DigitalContent.Configuration;
using Digital_Content.DigitalContent.Debugging;
using Digital_Content.DigitalContent.Emailing;
using Digital_Content.DigitalContent.Features;
using Digital_Content.DigitalContent.Friendships;
using Digital_Content.DigitalContent.Friendships.Cache;
using Digital_Content.DigitalContent.Localization;
using Digital_Content.DigitalContent.MultiTenancy;
using Digital_Content.DigitalContent.MultiTenancy.Payments.Cache;
using Digital_Content.DigitalContent.Notifications;
using Digital_Content.DigitalContent.Timing;
using Abp.Threading.BackgroundWorkers;
using Digital_Content.DigitalContent.ExchangeRates;
using Digital_Content.DigitalContent.Currencies;

#if FEATURE_LDAP
using Abp.Zero.Ldap;
#endif

namespace Digital_Content.DigitalContent
{
    [DependsOn(
        typeof(AbpZeroCoreModule),
#if FEATURE_LDAP
        typeof(AbpZeroLdapModule),
#endif
        typeof(AbpAutoMapperModule),
        typeof(AbpMailKitModule))]
    public class DigitalContentCoreModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Auditing.IsEnabledForAnonymousUsers = true;

            //Declare entity types
            Configuration.Modules.Zero().EntityTypes.Tenant = typeof(Tenant);
            Configuration.Modules.Zero().EntityTypes.Role = typeof(Role);
            Configuration.Modules.Zero().EntityTypes.User = typeof(User);

            DigitalContentLocalizationConfigurer.Configure(Configuration.Localization);

            //Adding feature providers
            Configuration.Features.Providers.Add<AppFeatureProvider>();

            //Adding setting providers
            Configuration.Settings.Providers.Add<AppSettingProvider>();

            //Adding notification providers
            Configuration.Notifications.Providers.Add<AppNotificationProvider>();

            //Enable this line to create a multi-tenant application.
            Configuration.MultiTenancy.IsEnabled = DigitalContentConsts.MultiTenancyEnabled;
            
            //Enable LDAP authentication (It can be enabled only if MultiTenancy is disabled!)
            //Configuration.Modules.ZeroLdap().Enable(typeof(AppLdapAuthenticationSource));

            //Configure roles
            AppRoleConfig.Configure(Configuration.Modules.Zero().RoleManagement);

            if (DebugHelper.IsDebug)
            {
                //Disabling email sending in debug mode
                Configuration.ReplaceService<IEmailSender, NullEmailSender>(DependencyLifeStyle.Transient);
            }

            Configuration.ReplaceService(typeof(IEmailSenderConfiguration), () =>
            {
                Configuration.IocManager.IocContainer.Register(
                    Component.For<IEmailSenderConfiguration, ISmtpEmailSenderConfiguration>()
                             .ImplementedBy<DigitalContentSmtpEmailSenderConfiguration>()
                             .LifestyleTransient()
                );
            });

            Configuration.Caching.Configure(FriendCacheItem.CacheName, cache =>
            {
                cache.DefaultSlidingExpireTime = TimeSpan.FromMinutes(30);
            });

            Configuration.Caching.Configure(PaymentCacheItem.CacheName, cache =>
            {
                cache.DefaultSlidingExpireTime = TimeSpan.FromMinutes(DigitalContentConsts.PaymentCacheDurationInMinutes);
            });
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(DigitalContentCoreModule).GetAssembly());
        }

        public override void PostInitialize()
        {
            IocManager.RegisterIfNot<IChatCommunicator, NullChatCommunicator>();

            IocManager.Resolve<ChatUserStateWatcher>().Initialize();
            IocManager.Resolve<AppTimes>().StartupTime = Clock.Now;
            var workManager = IocManager.Resolve<IBackgroundWorkerManager>();
            workManager.Add(IocManager.Resolve<UpdateTempCurrenciesWorker>());
            workManager.Add(IocManager.Resolve<UpdateCryptoCurrenciesWorker>());
            workManager.Add(IocManager.Resolve<UpdateExchangeRatesWorker>());      
        }
    }
}