using Abp.Authorization.Users;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Linq;
using Digital_Content.DigitalContent.Authorization.Roles;

namespace Digital_Content.DigitalContent.Authorization.Users
{
    /// <summary>
    /// Used to perform database operations for <see cref="IUserManager"/>.
    /// </summary>
    public class UserStore : AbpUserStore<Role, User>
    {
        public UserStore(
            IRepository<User, long> userRepository,
            IRepository<UserLogin, long> userLoginRepository,
            IRepository<UserRole, long> userRoleRepository,
            IRepository<Role> roleRepository,
            IAsyncQueryableExecuter asyncQueryableExecuter, 
            IUnitOfWorkManager unitOfWorkManager,
            IRepository<UserClaim, long> userCliamRepository,
            IRepository<UserPermissionSetting, long> userPermissionSettingRepository)
            : base(
                unitOfWorkManager,
                userRepository,
                roleRepository,
                asyncQueryableExecuter,
                userRoleRepository,
                userLoginRepository,
                userCliamRepository,
                userPermissionSettingRepository)
        {
        }
    }
}