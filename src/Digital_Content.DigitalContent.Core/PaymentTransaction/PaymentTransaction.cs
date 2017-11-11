using Digital_Content.DigitalContent.Wallets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Content.DigitalContent.PaymentTransaction
{ 
    public class PaymentTransaction
    {
        public long PaymentId { get; set; }
        public Payments.Payment Payment { get; set; }

        public long TransactionId { get; set; }
        public Transaction Transaction { get; set; }
 
    }
}
