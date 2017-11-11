using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Digital_Content.DigitalContent.Authorization.Users.Dto;

namespace Digital_Content.DigitalContent.Authorization.Users
{
    public interface IUserLoginAppService : IApplicationService
    {
        Task<ListResultDto<UserLoginAttemptDto>> GetRecentUserLoginAttempts();
    }
}
