using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Digital_Content.DigitalContent.Roadmap.Dto;

namespace Digital_Content.DigitalContent.Roadmap
{
    public interface IRoadmapAppService : IApplicationService
    {
        Task<PagedResultDto<RoadmapRecordDto>> GetRecords(GetRecordsInput input);

        Task AddRecord(RoadmapRecordDtoInput input);

        Task RemoveRecord(RoadmapRecordRemoveDtoInput input);

        Task UpdateRecord(RoadmapRecordUpdateDtoInput input);
    }
}