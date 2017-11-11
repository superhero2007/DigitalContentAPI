using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Auditing;
using Abp.MultiTenancy;
using Abp.Runtime.Session;
using Microsoft.EntityFrameworkCore;
using Digital_Content.DigitalContent.Chat.SignalR;
using Digital_Content.DigitalContent.Editions;
using Digital_Content.DigitalContent.Sessions.Dto;
using Digital_Content.DigitalContent.TermsOfServices.Dto;
using Abp.Domain.Repositories;
using Digital_Content.DigitalContent.Authorization.Users;
using Abp.UI;

namespace Digital_Content.DigitalContent.Sessions
{
    public class SessionAppService : DigitalContentAppServiceBase, ISessionAppService
    {
        private readonly IRepository<TermsOfServices.TermsOfService, long> _termsOfServiceRepository;
        private readonly IRepository<User, long> _userRepository;

        public SessionAppService(IRepository<TermsOfServices.TermsOfService, long> termsOfServiceRepository,
                                 IRepository<User, long> userRepository)
        {
            _termsOfServiceRepository = termsOfServiceRepository;
            _userRepository = userRepository;
        }


        [DisableAuditing]
        public async Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations()
        {
            var output = new GetCurrentLoginInformationsOutput
            {
                Application = new ApplicationInfoDto
                {
                    Version = AppVersionHelper.Version,
                    ReleaseDate = AppVersionHelper.ReleaseDate,
                    Features = new Dictionary<string, bool>
                    {
                        { "SignalR", SignalRFeature.IsAvailable }
                    }
                }
            };

            if (AbpSession.TenantId.HasValue)
            {
                output.Tenant = ObjectMapper
                                    .Map<TenantLoginInfoDto>(await TenantManager
                                        .Tenants
                                        .Include(t => t.Edition)
                                        .FirstAsync(t => t.Id == AbpSession.GetTenantId()));
            }

            

            if (AbpSession.UserId.HasValue)
            {
                output.User = ObjectMapper.Map<UserLoginInfoDto>(await GetCurrentUserAsync());
            }
            output.Tos = await AcceptLatestVersionOfTosAsync();
            if (output.Tenant == null)
            {
                return output;
            }

            

            output.Tenant.Edition?.SetEditionIsHighest(GetTopEditionOrNullByMonthlyPrice());
            output.Tenant.SubscriptionDateString = GetTenantSubscriptionDateString(output);
            output.Tenant.CreationTimeString = output.Tenant.CreationTime.ToString("d");
           

            return output;
        }

        private SubscribableEdition GetTopEditionOrNullByMonthlyPrice()
        {
            var editions = TenantManager.EditionManager.Editions;
            if (editions == null || !editions.Any())
            {
                return null;
            }

            return ObjectMapper
                  .Map<IEnumerable<SubscribableEdition>>(editions)
                  .OrderByDescending(e => e.MonthlyPrice)
                  .FirstOrDefault();
        }

        private string GetTenantSubscriptionDateString(GetCurrentLoginInformationsOutput output)
        {
            return output.Tenant.SubscriptionEndDateUtc == null
                ? L("Unlimited")
                : output.Tenant.SubscriptionEndDateUtc?.ToString("d");
        }

        public async Task<UpdateUserSignInTokenOutput> UpdateUserSignInToken()
        {
            if (AbpSession.UserId <= 0)
            {
                throw new Exception(L("ThereIsNoLoggedInUser"));
            }

            var user = await UserManager.GetUserAsync(AbpSession.ToUserIdentifier());
            user.SetSignInToken();
            return new UpdateUserSignInTokenOutput
            {
                SignInToken = user.SignInToken,
                EncodedUserId = Convert.ToBase64String(Encoding.UTF8.GetBytes(user.Id.ToString())),
                EncodedTenantId = user.TenantId.HasValue ? Convert.ToBase64String(Encoding.UTF8.GetBytes(user.TenantId.Value.ToString())) : ""
            };
        }


        private async Task<TermsOfServiceOutput> AcceptLatestVersionOfTosAsync()
        {
            try
            {
                var user = await _userRepository.FirstOrDefaultAsync(item => item.Id == AbpSession.UserId);
                var defaultTos = await _termsOfServiceRepository.FirstOrDefaultAsync(tos => tos.IsDefault == true);
                if (defaultTos != null && user != null)
                {
                    if (defaultTos.Version == user.TosVersion)
                    {
                        return new TermsOfServiceOutput
                        {
                            IsLattest = true,
                            TosContent = defaultTos.TosContent
                        };
                    }
                    else
                    {
                        return new TermsOfServiceOutput
                        {
                            IsLattest = false,
                            TosContent = defaultTos.TosContent
                        };
                    }
                }
                else
                {
                    return new TermsOfServiceOutput
                    {
                        IsLattest = true,
                        TosContent = String.Empty
                    };
                }

            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }

        }

        public async Task<AcceptTosOutput> UpdateUSerTosVersion()
        {
            try
            {
                var user = await _userRepository.FirstOrDefaultAsync(item => item.Id == AbpSession.UserId);
                var defaultTos = await _termsOfServiceRepository.FirstOrDefaultAsync(tos => tos.IsDefault == true);
                if (user != null && defaultTos != null)
                {
                    user.TosVersion = defaultTos.Version;
                    await _userRepository.UpdateAsync(user);
                    return new AcceptTosOutput
                    {
                        IsSuccess = true
                    };
                }
                else
                {
                    return new AcceptTosOutput
                    {
                        IsSuccess = false
                    };
                }
            }
            catch(Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
    }
}