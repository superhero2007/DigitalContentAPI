using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Content.DigitalContent.Wallet.Dto
{
    public class PaymentOutput
    {
        public string QrCode { get; set; }

        public string Instruction { get; set; }

        public string Address { get; set; }
    }
}
