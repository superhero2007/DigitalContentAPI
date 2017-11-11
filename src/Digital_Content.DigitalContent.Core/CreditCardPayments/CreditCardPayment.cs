using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Digital_Content.DigitalContent.Authorization.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Content.DigitalContent.CreditCardPayments
{
    [Table("CreditCardPaymentLogs")]
    public class CreditCardPayment : Entity<long>, IHasCreationTime,IMustHaveTenant
    {
        public long Amount { get; set; }

        public string BalanceTransactionId { get; set; }

        public DateTime CreationTime { get; set; }

        public string Currency { get; set; }

        public string Description { get; set; }

        public string ChargeId { get; set; }

        public string Status { get; set; }

        public PaymentOutcome Outcome { get; set; }

        public PaymentSource Source { get; set; }


        [ForeignKey("CreditCardUserId")]
        public virtual User User { get; set; }

        public long CreditCardUserId { get; set; }
        public int TenantId { get; set; }
    }
}

