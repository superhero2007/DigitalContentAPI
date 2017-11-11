using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Content.DigitalContent.CreditCardPayments
{
    
    [Table("Outcome")]
    public class PaymentOutcome : Entity<long>, IMustHaveTenant
    {
        public string NetworkStatus { get; set; }

        public string RiskLevel { get; set; }

        public string SellerMessage { get; set; }

        public string Type { get; set; }
        public int TenantId { get; set; }
    }
}
