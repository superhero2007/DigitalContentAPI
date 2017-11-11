using System.Collections.Generic;
using Digital_Content.DigitalContent.Authorization.Permissions.Dto;

namespace Digital_Content.DigitalContent.Authorization.Users.Dto
{
    public class GetUserPermissionsForEditOutput
    {
        public List<FlatPermissionDto> Permissions { get; set; }

        public List<string> GrantedPermissionNames { get; set; }
    }
}