using System;
using System.Linq;
using Abp.AspNetCore;
using Abp.Castle.Logging.Log4Net;
using Abp.Dependency;
using Abp.Extensions;
using Abp.Hangfire;
using Abp.Timing;
using Castle.Facilities.Logging;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Cors.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Digital_Content.DigitalContent.Authorization;
using Digital_Content.DigitalContent.Authorization.Roles;
using Digital_Content.DigitalContent.Authorization.Users;
using Digital_Content.DigitalContent.Configuration;
using Digital_Content.DigitalContent.Identity;
using Digital_Content.DigitalContent.MultiTenancy;
using PaulMiami.AspNetCore.Mvc.Recaptcha;
using Swashbuckle.AspNetCore.Swagger;
using Digital_Content.DigitalContent.Web.IdentityServer;
using System.Configuration;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Digital_Content.DigitalContent.WorkflowEngine.src;
using Digital_Content.DigitalContent.ExchangeRates;
using Digital_Content.DigitalContent.CreditCardPayments;

#if FEATURE_SIGNALR
using Digital_Content.DigitalContent.Web.Owin;
using Abp.Owin;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Cors;
using Owin;
using Owin.Security.AesDataProtectorProvider;
using Abp.Web.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
#endif

namespace Digital_Content.DigitalContent.Web.Startup
{
    public class Startup
    {


#region Properties

        private const string DefaultCorsPolicyName = "localhost";

        private readonly IConfigurationRoot _appConfiguration;
        


        #endregion


        public Startup(IHostingEnvironment env)
        {
            _appConfiguration = env.GetAppConfiguration();
            IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Environment.CurrentDirectory)
            .AddJsonFile("appsettings.json")
            .Build();
            WorkflowInit.ConnectionString = configuration.GetConnectionString("Default");
            UserEmailer.SendgridApikey = configuration.GetValue<string>("SENDGRID_APIKEY");
            UserEmailer.FromName = configuration.GetValue<string>("FROM_NAME");
            UserEmailer.FromEmail = configuration.GetValue<string>("FROM_EMAIL");
            UserEmailer.EmailSubject = configuration.GetValue<string>("EMAIL_SUBJECT");
            CreditCardPaymentAppService.StripeApiKey = configuration.GetValue<string>("STRIPE_API_KEY");


        }


        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            
            //MVC
            services.AddMvc(options =>
            {
                options.Filters.Add(new CorsAuthorizationFilterFactory(DefaultCorsPolicyName));
            });
            services.AddSingleton<IConfiguration>(_appConfiguration);

            //Configure CORS for angular2 UI
            services.AddCors(options =>
            {
                options.AddPolicy(DefaultCorsPolicyName, builder =>
                {
                    //App:CorsOrigins in appsettings.json can contain more than one address with splitted by comma.
                    builder
                        //.WithOrigins(_appConfiguration["App:CorsOrigins"].Split(",", StringSplitOptions.RemoveEmptyEntries).Select(o => o.RemovePostFix("/")).ToArray())
                        .AllowAnyOrigin() //TODO: Will be replaced by above when Microsoft releases microsoft.aspnetcore.cors 2.0 - https://github.com/aspnet/CORS/pull/94
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

            IdentityRegistrar.Register(services, "Host");

            //Identity server
            if (bool.Parse(_appConfiguration["IdentityServer:IsEnabled"]))
            {
                IdentityServerRegistrar.Register(services, _appConfiguration);
            }

            //Swagger - Enable this line and the related lines in Configure method to enable swagger UI
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Info { Title = "DigitalContent API", Version = "v1" });
                options.DocInclusionPredicate((docName, description) => true);
            });

            //Recaptcha
            services.AddRecaptcha(new RecaptchaOptions
            {
                SiteKey = _appConfiguration["Recaptcha:SiteKey"],
                SecretKey = _appConfiguration["Recaptcha:SecretKey"]
            });

            //Hangfire (Enable to use Hangfire instead of default job manager)
            //services.AddHangfire(config =>
            //{
            //    config.UseSqlServerStorage(_appConfiguration.GetConnectionString("Default"));
            //});

            //Configure Abp and Dependency Injection
            return services.AddAbp<DigitalContentWebHostModule>(options =>
            {
                //Configure Log4Net logging
                options.IocManager.IocContainer.AddFacility<LoggingFacility>(
                    f => f.UseAbpLog4Net().WithConfig("log4net.config")
                );
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            //Initializes ABP framework.
            app.UseAbp(options =>
            {
                options.UseAbpRequestLocalization = false; //used below: UseAbpRequestLocalization
            });

            app.UseCors(DefaultCorsPolicyName); //Enable CORS!

            AuthConfigurer.Configure(app, _appConfiguration);

            app.UseStaticFiles();

            app.UseAbpRequestLocalization();

#if FEATURE_SIGNALR
            //Integrate to OWIN
            app.UseAppBuilder(ConfigureOwinServices);
#endif

            //Hangfire dashboard & server (Enable to use Hangfire instead of default job manager)
            //app.UseHangfireDashboard("/hangfire", new DashboardOptions
            //{
            //    Authorization = new[] { new AbpHangfireAuthorizationFilter(AppPermissions.Pages_Administration_HangfireDashboard)  }
            //});
            //app.UseHangfireServer();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "defaultWithArea",
                    template: "{area}/{controller=Home}/{action=Index}/{id?}");

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            // Enable middleware to serve generated Swagger as a JSON endpoint
            app.UseSwagger();
            // Enable middleware to serve swagger-ui assets (HTML, JS, CSS etc.)
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "DigitalContent API V1");
            }); //URL: /swagger
        }

#if FEATURE_SIGNALR
        private static void ConfigureOwinServices(IAppBuilder app)
        {
            GlobalHost.DependencyResolver.Register(typeof(IAssemblyLocator), () => new SignalRAssemblyLocator());
            app.Properties["host.AppName"] = "DigitalContent";

            app.UseAbp();
            app.UseAesDataProtectorProvider();

            app.Map("/signalr", map =>
            {
                map.UseCors(CorsOptions.AllowAll);

                var hubConfiguration = new HubConfiguration
                {
                    EnableJSONP = true
                };

                map.RunSignalR(hubConfiguration);
            });
        }
#endif
    }
}
