using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Digital_Content.DigitalContent.Currencies;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Content.DigitalContent.ExchangeRates
{
    public class ExchangeRate : Entity<long>, IHasCreationTime
    {
        public DateTime CreationTime { get; set; }

        public Guid CurrencyId { get; set; }

        //public int Currency { get; set; } //TODO: old

        public decimal Price { get; set; }

        public bool IsAutoWallet { get; set; }

        public bool IsAutomaticUpdate { get; set; }

        public long? UpdatedBy { get; set; }

        public Guid? DataSourceId { get; set; }
    }
}
