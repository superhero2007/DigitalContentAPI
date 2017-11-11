using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Digital_Content.DigitalContent.Editions.Dto;

namespace Digital_Content.DigitalContent.Editions
{
    public interface IEditionAppService : IApplicationService
    {
        Task<ListResultDto<EditionListDto>> GetEditions();

        Task<GetEditionEditOutput> GetEditionForEdit(NullableIdDto input);

        Task CreateOrUpdateEdition(CreateOrUpdateEditionDto input);

        Task DeleteEdition(EntityDto input);

        Task<List<SubscribableEditionComboboxItemDto>> GetEditionComboboxItems(int? selectedEditionId = null, bool addAllItem = false, bool onlyFree = false);
    }
}