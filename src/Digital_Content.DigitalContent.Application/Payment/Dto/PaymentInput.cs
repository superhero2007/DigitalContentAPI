using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Content.DigitalContent.Payment.Dto
{
    public class PaymentInput
    {
        public decimal AmountOfFunds { get; set; } 
        public DateTime PaymentReceivedTime { get; set; }
        //TODO: old public string CurrencyType { get; set; }
        public Guid CurrencyId { get; set; }
        public string WalletId { get; set; }
        public int TrackingNumber { get; set; }
        public long? TransactionId { get; set; }
        public long TokensIssued { get; set; }
    }
}
