using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Content.DigitalContent.CreditCardPayments
{
    [Table("Card")]
    public class CreditCard : Entity<long>
    {
        public string CardId { get; set; }

        public string Brand { get; set; }

        public string Country { get; set; }

        public int ExpirationMonth { get; set; }

        public long ExpirationYear { get; set; }

        public string FingerPrint { get; set; }

        public string Funding { get; set; }

        public string Last4 { get; set; }
    }
}
