using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Content.DigitalContent.Dto
{
    public class DcntEmailTemplateInput
    {
        public string UserName { get; set; }

        public decimal Dcnt { get; set; }

        public string Uri { get; set; }

        public decimal Dollar { get; set; }

        public decimal Amount { get; set; }

        public string CurrentType { get; set; }

        public decimal DcntInBtcOrEthOrUsd { get; set; }

        public string WalletId { get; set; }

        public string QrCode { get; set; }
    }
}
