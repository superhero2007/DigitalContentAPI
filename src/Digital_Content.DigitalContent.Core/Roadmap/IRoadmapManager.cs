using Abp.Domain.Services;
using Digital_Content.DigitalContent.Roadmap.Entity;
using System.Threading.Tasks;

namespace Digital_Content.DigitalContent.Roadmap
{
    public interface IRoadmapManager : IDomainService
    {
        Task AddRecordAsync(RoadmapRecord roadmapRecord);

        Task RemoveRecordAsync(long id);

        Task UpdateRecordAsync(RoadmapRecord roadmapRecord);
    }
}
