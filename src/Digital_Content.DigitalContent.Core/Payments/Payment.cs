using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Digital_Content.DigitalContent.Wallets;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Content.DigitalContent.Payments
{
    public class Payment : Entity<long>, IHasCreationTime, IMayHaveTenant
    {
        public decimal AmountOfFunds { get; set; }

        public DateTime CreationTime { get; set; }

        public DateTime PaymentReceivedTime { get; set; }

        //TODO old public CurrentTypeEnum CurrencyType { get; set; }

        public Guid CurrencyId { get; set; }

        public string WalletId { get; set; }

        public int? TrackingNumber { get; set; }

 
        public virtual ICollection<PaymentTransaction.PaymentTransaction> PaymentTransaction { get; set; }
        public int? TenantId { get; set; }

        public Payment()
        {
            PaymentTransaction = new List<PaymentTransaction.PaymentTransaction>();
        }
    }
}
