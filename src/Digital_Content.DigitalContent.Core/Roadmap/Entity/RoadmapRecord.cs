using System;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations.Schema;

namespace Digital_Content.DigitalContent.Roadmap.Entity
{
    public class RoadmapRecord : Entity<long>, IHasCreationTime, IMayHaveTenant    
    {
        public DateTime CreationTime { get; set; }

        public string Title { get; set; }

        public DateTime Date { get; set; }

        public string Description { get; set; }

        public decimal ExchangeRate { get; set; }

        public long MinTokenToBuy { get; set; }

        public decimal Bonus { get; set; }

        public bool IncludeFeatureYears { get; set; }

        public int? TenantId { get; set; }


        public RoadmapRecord()
        {
            CreationTime = DateTime.UtcNow;
        }
    }
}