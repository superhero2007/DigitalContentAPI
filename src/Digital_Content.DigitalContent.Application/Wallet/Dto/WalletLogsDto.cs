using Digital_Content.DigitalContent.Wallets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Content.DigitalContent.Wallet.Dto
{
    public class WalletLogsDto
    {
        public string WalletId { get; set; }

        public long UserId { get; set; }

        public decimal Amount { get; set; }

        public string Address { get; set; }

        //public string CurrentType { get; set; } //TODO: old
        
        public Guid CurrencyId { get; set; }
    }
}
