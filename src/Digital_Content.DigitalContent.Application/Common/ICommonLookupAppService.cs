using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Digital_Content.DigitalContent.Common.Dto;
using Digital_Content.DigitalContent.Editions.Dto;

namespace Digital_Content.DigitalContent.Common
{
    public interface ICommonLookupAppService : IApplicationService
    {
        Task<ListResultDto<SubscribableEditionComboboxItemDto>> GetEditionsForCombobox(bool onlyFreeItems = false);

        Task<PagedResultDto<NameValueDto>> FindUsers(FindUsersInput input);

        GetDefaultEditionNameOutput GetDefaultEditionName();
    }
}