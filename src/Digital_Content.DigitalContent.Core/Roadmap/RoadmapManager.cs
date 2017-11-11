using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Digital_Content.DigitalContent.Roadmap.Entity;
using System.Threading.Tasks;

namespace Digital_Content.DigitalContent.Roadmap
{
    public class RoadmapManager : IRoadmapManager
    {
        private readonly IRepository<RoadmapRecord, long> _roadmapRecordRepository;

        public RoadmapManager(IRepository<RoadmapRecord, long> roadmapRecordRepository)
        {
            _roadmapRecordRepository = roadmapRecordRepository;
        }

        [UnitOfWork]
        public async Task AddRecordAsync(RoadmapRecord roadmapRecord)
        {
            await _roadmapRecordRepository.InsertAsync(roadmapRecord);
        }

        [UnitOfWork]
        public async Task RemoveRecordAsync(long id)
        {
            await _roadmapRecordRepository.DeleteAsync(id);
        }

        [UnitOfWork]
        public async Task UpdateRecordAsync(RoadmapRecord roadmapRecord)
        {
            await _roadmapRecordRepository.UpdateAsync(roadmapRecord);
        }
    }
}
