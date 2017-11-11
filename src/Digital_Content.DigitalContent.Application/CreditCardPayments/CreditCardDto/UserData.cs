using Digital_Content.DigitalContent.Wallet.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Content.DigitalContent.CreditCardPayments.CreditCardDto
{
    public class UserData
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Address1 { get; set; }

        public string Address2 { get; set; }

        public string City { get; set; }

        public string Country { get; set; }

        public string State { get; set; }

        public string ZipCode { get; set; }

        public string PhoneNumber { get; set; }

        public string NameCard { get; set; }
         
        public WalletLogsDto UserPaymentData { get; set; }

    }
}
