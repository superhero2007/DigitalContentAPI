using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Content.DigitalContent.CreditCardPayments
{
    [Table("Source")]
    public class PaymentSource : Entity<long>,IMayHaveTenant
    {
        public string SourceId { get; set; }

        public string BankAccount { get; set; }

        public CreditCard Card { get; set; }

        public int? TenantId { get; set; }
    }
}
