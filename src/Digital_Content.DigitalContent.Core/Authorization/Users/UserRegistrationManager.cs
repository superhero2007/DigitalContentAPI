using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Authorization.Users;
using Abp.Configuration;
using Abp.IdentityFramework;
using Abp.Notifications;
using Abp.Runtime.Session;
using Abp.UI;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Digital_Content.DigitalContent.Authorization.Roles;
using Digital_Content.DigitalContent.Configuration;
using Digital_Content.DigitalContent.Debugging;
using Digital_Content.DigitalContent.MultiTenancy;
using Digital_Content.DigitalContent.Notifications;
using Digital_Content.DigitalContent.Authorization.Companies;
using Digital_Content.DigitalContent.Authorization.Companies.Dto;
using Digital_Content.DigitalContent.TermsOfServices;
using Abp.Domain.Repositories;
using Digital_Content.DigitalContent.Friendships;

namespace Digital_Content.DigitalContent.Authorization.Users
{
    public class UserRegistrationManager : DigitalContentDomainServiceBase
    {
        public IAbpSession AbpSession { get; set; }

        private readonly TenantManager _tenantManager;
        private readonly IUserManager _userManager;
        private readonly RoleManager _roleManager;
        private readonly IUserEmailer _userEmailer;
        private readonly INotificationSubscriptionManager _notificationSubscriptionManager;
        private readonly IAppNotifier _appNotifier;
        private readonly IUserPolicy _userPolicy;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly ICompanyManager _companyManager;
        private readonly IRepository<TermsOfService, long> _termsOfServicesRepository;
        private readonly IRepository<Friendship, long> _friendshipRepository;



        public UserRegistrationManager(
            TenantManager tenantManager,
            IUserManager userManager,
            RoleManager roleManager,
            IUserEmailer userEmailer,
            INotificationSubscriptionManager notificationSubscriptionManager,
            IAppNotifier appNotifier,
            IUserPolicy userPolicy,
            IPasswordHasher<User> passwordHasher,
            CompanyManager companyManager,
            IRepository<TermsOfService, long> termsOfServicesRepository,
            IRepository<Friendship, long> friendshipRepository)
        {
            _tenantManager = tenantManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _userEmailer = userEmailer;
            _notificationSubscriptionManager = notificationSubscriptionManager;
            _appNotifier = appNotifier;
            _userPolicy = userPolicy;
            _passwordHasher = passwordHasher;
            _companyManager = companyManager;
            AbpSession = NullAbpSession.Instance;
            _termsOfServicesRepository = termsOfServicesRepository;
            _friendshipRepository = friendshipRepository;
        }

        public async Task<User> RegisterAsync(
                string firstName,
                string lastName,
                string userName,
                string yourName,
                string contentType,
                string contentVolume,
                string industry,
                bool isContentShedule,
                string cms,
                string website,
                string company,
                string userType,
                string phoneNumber,
                string emailAddress,
                string plainPassword,
                bool isEmailConfirmed)
        {
            //CheckForTenant();
            //CheckSelfRegistrationIsEnabled();

            //var tenant = 1;await GetActiveTenantAsync();
            //var isNewRegisteredUserActiveByDefault = await SettingManager.GetSettingValueAsync<bool>(AppSettings.UserManagement.IsNewRegisteredUserActiveByDefault);
            try
            {
                var companyName = new CompanyInput
                {
                    Name = company,
                    CreationTime = DateTime.Now
                };
                var companyId = await _companyManager.CreateCompany(companyName);

                //await _userPolicy.CheckMaxUserCountAsync(tenant.Id);


                var user = new User
                {
                    EmailAddress = emailAddress,
                    IsEmailConfirmed = false,
                    Name = firstName,
                    Surname = lastName,
                    UserName = userName ?? firstName + emailAddress,
                    UserType = (int)Enum.Parse(typeof(UserType), userType),
                    PhoneNumber = phoneNumber,
                    CompanyUserId = companyId,
                    YourName = yourName,
                    Website = website,
                    ContentType = contentType,
                    ContentVolume = contentVolume,
                    Industry = industry,
                    IsContentShedule = isContentShedule,
                    CMS = cms,
                    IsActive = false,
                    Roles = new List<UserRole>(),
                    TosVersion = 0,
                    TenantId = AbpSession.TenantId
                };

                //user.SetNormalizedNames();

                user.Password = _passwordHasher.HashPassword(user, plainPassword);

                foreach (var defaultRole in await _roleManager.Roles.Where(r => r.IsDefault).ToListAsync())
                {
                    user.Roles.Add(new UserRole(null, user.Id, defaultRole.Id));
                }

                await _userManager.CreateAsync(user);
                await CurrentUnitOfWork.SaveChangesAsync();

                //if (!user.IsEmailConfirmed)
                //{
                //    user.SetNewEmailConfirmationCode();
                //await _userEmailer.SendEmailActivationLinkAsync(user, emailActivationLink);
                //}

                //Notifications
                _friendshipRepository.Insert(new Friendship(new Abp.UserIdentifier(AbpSession.TenantId, user.Id), new Abp.UserIdentifier(null, 1), "Default", "admin", null, FriendshipState.Accepted));
                var curentTenant = await GetActiveTenantAsync();
                var tenantAdmin  = (await _userManager.GetUsersInRoleAsync("Admin")).FirstOrDefault();
                if (tenantAdmin != null)
                {
                    _friendshipRepository.Insert(new Friendship(new Abp.UserIdentifier(AbpSession.TenantId, user.Id), new Abp.UserIdentifier(curentTenant.Id, tenantAdmin.Id), curentTenant.TenancyName, tenantAdmin.Name, null, FriendshipState.Accepted));
                }
                await _notificationSubscriptionManager.SubscribeToAllAvailableNotificationsAsync(user.ToUserIdentifier());
                await _appNotifier.WelcomeToTheApplicationAsync(user);
                await _appNotifier.NewUserRegisteredAsync(user);

                return user;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        private void CheckForTenant()
        {
            if (!AbpSession.TenantId.HasValue)
            {
                throw new InvalidOperationException("Can not register host users!");
            }
        }

        private void CheckSelfRegistrationIsEnabled()
        {
            if (!SettingManager.GetSettingValue<bool>(AppSettings.UserManagement.AllowSelfRegistration))
            {
                throw new UserFriendlyException(L("SelfUserRegistrationIsDisabledMessage_Detail"));
            }
        }

        private bool UseCaptchaOnRegistration()
        {
            if (DebugHelper.IsDebug)
            {
                return false;
            }

            return SettingManager.GetSettingValue<bool>(AppSettings.UserManagement.UseCaptchaOnRegistration);
        }

        private async Task<Tenant> GetActiveTenantAsync()
        {
            if (!AbpSession.TenantId.HasValue)
            {
                return null;
            }

            return await GetActiveTenantAsync(AbpSession.TenantId.Value);
        }

        private async Task<Tenant> GetActiveTenantAsync(int tenantId)
        {
            var tenant = await _tenantManager.FindByIdAsync(tenantId);
            if (tenant == null)
            {
                throw new UserFriendlyException(L("UnknownTenantId{0}", tenantId));
            }

            if (!tenant.IsActive)
            {
                throw new UserFriendlyException(L("TenantIdIsNotActive{0}", tenantId));
            }

            return tenant;
        }

        protected virtual void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }
    }
}
