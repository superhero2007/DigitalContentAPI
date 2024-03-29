﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Authorization.Roles;
using Abp.Domain.Uow;
using Abp.Localization;
using Abp.Runtime.Caching;
using Abp.UI;
using Abp.Zero.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Digital_Content.DigitalContent.Authorization.Users;

namespace Digital_Content.DigitalContent.Authorization.Roles
{
    /// <summary>
    /// Role manager.
    /// Used to implement domain logic for roles.
    /// </summary>
    public class RoleManager : AbpRoleManager<Role, User>
    {
        private ILocalizationManager _localizationManager;

        public RoleManager(
            RoleStore store,
            IEnumerable<IRoleValidator<Role>> roleValidators,
            ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors,
            ILogger<RoleManager> logger,
            IHttpContextAccessor contextAccessor,
            IPermissionManager permissionManager,
            IRoleManagementConfig roleManagementConfig,
            ICacheManager cacheManager,
            IUnitOfWorkManager unitOfWorkManager,
            ILocalizationManager localizationManager)
            : base(
                store,
                roleValidators,
                keyNormalizer,
                errors,
                logger,
                contextAccessor,
                permissionManager,
                cacheManager,
                unitOfWorkManager,
                roleManagementConfig)
        {
            _localizationManager = localizationManager;
        }

        public override Task SetGrantedPermissionsAsync(Role role, IEnumerable<Permission> permissions)
        {
            CheckPermissionsToUpdate(role, permissions);

            return base.SetGrantedPermissionsAsync(role, permissions);
        }

        private void CheckPermissionsToUpdate(Role role, IEnumerable<Permission> permissions)
        {
            if (role.Name == StaticRoleNames.Host.Admin &&
                (!permissions.Any(p => p.Name == AppPermissions.Pages_Administration_Roles_Edit) ||
                 !permissions.Any(p => p.Name == AppPermissions.Pages_Administration_Users_ChangePermissions)))
            {
                throw new UserFriendlyException(L("YouCannotRemoveUserRolePermissionsFromAdminRole"));
            }
        }

        private string L(string name)
        {
            return _localizationManager.GetString(DigitalContentConsts.LocalizationSourceName, name);
        }
    }
}