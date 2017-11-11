using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Digital_Content.DigitalContent.Roadmap.Dto;
using Abp.Domain.Repositories;
using Digital_Content.DigitalContent.Roadmap.Entity;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using System;
using Abp.UI;
using System.ComponentModel.DataAnnotations;

namespace Digital_Content.DigitalContent.Roadmap
{
    public class RoadmapAppService : DigitalContentAppServiceBase, IRoadmapAppService
    {
        private readonly IRepository<RoadmapRecord, long> _roadmapRecordRepository;
        private readonly IRoadmapManager _roadmapManager;

        public RoadmapAppService(IRepository<RoadmapRecord, long> roadmapRecordRepository, IRoadmapManager roadmapManager)
        {
            _roadmapRecordRepository = roadmapRecordRepository;
            _roadmapManager = roadmapManager;
        }

        public async Task<PagedResultDto<RoadmapRecordDto>> GetRecords(GetRecordsInput input)
        {
            var roadmapRecordsQueryable = _roadmapRecordRepository.GetAll();

            roadmapRecordsQueryable = roadmapRecordsQueryable.WhereIf(!input.Filter.IsNullOrWhiteSpace(), r => r.Title.Contains(input.Filter));

            var roadmapRecordsCount = await roadmapRecordsQueryable.CountAsync();

            var roadmapRecords = await roadmapRecordsQueryable.OrderBy(input.Sorting).PageBy(input).ToListAsync();

            return new PagedResultDto<RoadmapRecordDto>(roadmapRecordsCount, ObjectMapper.Map<List<RoadmapRecordDto>>(roadmapRecords));
        }

        public async Task AddRecord(RoadmapRecordDtoInput input)
        {
            var roadmapRecord = new RoadmapRecord
            {
                Title = input.Title,
                Date = input.Date,
                Description = input.Description,
                ExchangeRate = input.ExchangeRate,
                MinTokenToBuy = input.MinTokenToBuy,
                Bonus = input.Bonus,
                IncludeFeatureYears = input.IncludeFeatureYears,
                TenantId = AbpSession.TenantId
            };

            await _roadmapManager.AddRecordAsync(roadmapRecord);
        }

        public async Task RemoveRecord(RoadmapRecordRemoveDtoInput input)
        {
            var roadmapRecordDb = await _roadmapRecordRepository.FirstOrDefaultAsync(input.Id);

            if (roadmapRecordDb == null)
            {
                throw new UserFriendlyException(L("RoadmapRecordNotFound"));
            }

            await _roadmapManager.RemoveRecordAsync(input.Id);
        }

        public async Task UpdateRecord(RoadmapRecordUpdateDtoInput input)
        {
            var roadmapRecord = await _roadmapRecordRepository.FirstOrDefaultAsync(input.Id);

            if (roadmapRecord == null)
            {
                throw new UserFriendlyException(L("RoadmapRecordNotFound"));
            }

            roadmapRecord.Id = input.Id;
            roadmapRecord.Title = input.Title;
            roadmapRecord.Date = input.Date;
            roadmapRecord.Description = input.Description;
            roadmapRecord.ExchangeRate = input.ExchangeRate;
            roadmapRecord.MinTokenToBuy = input.MinTokenToBuy;
            roadmapRecord.Bonus = input.Bonus;
            roadmapRecord.IncludeFeatureYears = input.IncludeFeatureYears;

            await _roadmapManager.UpdateRecordAsync(roadmapRecord);
        }
    }
}