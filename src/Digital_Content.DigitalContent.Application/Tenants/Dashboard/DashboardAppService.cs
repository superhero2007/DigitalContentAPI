using Abp.Authorization.Users;
using Abp.Domain.Repositories;
using Digital_Content.DigitalContent.Authorization.Roles;
using Digital_Content.DigitalContent.Authorization.Users;
using Digital_Content.DigitalContent.Tenants.Dashboard.Dto;
using Digital_Content.DigitalContent.Wallets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Content.DigitalContent.Tenants.Dashboard
{
    public class DashboardAppService:DigitalContentAppServiceBase, IDashboardAppService
    {
        private readonly IRepository<User, long> _userRepository;
        private readonly IRepository<Role> _roleRepository;
        private readonly IRepository<UserRole, long> _userRoleRepository;
        private readonly IRepository<Transaction, long> _transactionRepository;



        public DashboardAppService(IRepository<User, long> userRepository,
                                   IRepository<Role> roleRepository,
                                   IRepository<UserRole, long> userRoleRepository,
                                   IRepository<Transaction, long> transactionRepository)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _userRoleRepository = userRoleRepository;
            _transactionRepository = transactionRepository;
        }

        public async Task<DasboardDataDto> GetDashboardDataForUser( )
        {
            try
            {
                //implement tenants
                var user = await _userRepository.GetAsync((long)UserManager.AbpSession.UserId);
                var totalDcntInvestors = await _userRepository.GetAllListAsync();
                var totalTokensPurchased =  _userRepository.GetAll().Sum(x => x.AmountTokens);
                return new DasboardDataDto()
                {
                    UserDcntTokensPurchased = user.AmountTokens,
                    UserAffricateTokensEarned = user.AffricateTokensEarned,
                    UserDcntTokensBalance = user.DcntTokensBalance,
                    UserDownline = user.Downline,
                    UserDownlineTokenBough = user.DownlineTokenBough,
                    TotalDcntInvestors = totalDcntInvestors.Count,
                    TotalTokensPurchased = totalTokensPurchased/100.0M
                };
            }
            catch (Exception ex) {
                throw new Abp.UI.UserFriendlyException(ex.Message);
            }
        }
    }
}
