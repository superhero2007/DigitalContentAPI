using System.Threading.Tasks;
using Abp.Application.Services;
using Digital_Content.DigitalContent.Sessions.Dto;

namespace Digital_Content.DigitalContent.Sessions
{
    public interface ISessionAppService : IApplicationService
    {
        Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations();

        Task<UpdateUserSignInTokenOutput> UpdateUserSignInToken();

        Task<AcceptTosOutput> UpdateUSerTosVersion();
    }
}
