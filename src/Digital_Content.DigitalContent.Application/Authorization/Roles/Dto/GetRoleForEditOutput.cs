using System.Collections.Generic;
using Abp.Application.Services.Dto;
using Digital_Content.DigitalContent.Authorization.Permissions.Dto;

namespace Digital_Content.DigitalContent.Authorization.Roles.Dto
{
    public class GetRoleForEditOutput
    {
        public RoleEditDto Role { get; set; }

        public List<FlatPermissionDto> Permissions { get; set; }

        public List<string> GrantedPermissionNames { get; set; }
    }
}