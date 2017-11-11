using System;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace Digital_Content.DigitalContent.Currencies
{
    public class SupportedCurrency : Entity<long>, IHasCreationTime, IMayHaveTenant
    {
        public int? TenantId { get; set; }

        public DateTime CreationTime { get; set; }

        public Guid CurrencyId { get; set; }

        public string WalletId { get; set; }

        public string WiredInstruction { get; set; }

        public int? SortOrder { get; set; }
    }
}