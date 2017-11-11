using System;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace Digital_Content.DigitalContent.Currencies
{
    public class Currency : Entity<Guid>, IHasCreationTime
    {
        public DateTime CreationTime { get; set; }

        public string Name { get; set; }

        public string Code { get; set; }

        public string ImageUrl { get; set; }

        public bool IsCrypto { get; set; }

        public int? SortOrder { get; set; }
    }
}