using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Digital_Content.DigitalContent.Authorization.Permissions.Dto;

namespace Digital_Content.DigitalContent.Authorization.Permissions
{
    public interface IPermissionAppService : IApplicationService
    {
        ListResultDto<FlatPermissionWithLevelDto> GetAllPermissions();
    }
}
