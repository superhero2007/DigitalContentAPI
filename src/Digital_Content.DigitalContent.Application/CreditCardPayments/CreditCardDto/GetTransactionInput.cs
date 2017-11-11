using Abp.Runtime.Validation;
using Digital_Content.DigitalContent.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Content.DigitalContent.CreditCardPayments.CreditCardDto
{
    public class GetTransactionInput : PagedAndSortedInputDto, IShouldNormalize
    {
        public string WalletId { get; set; }

        public bool IsRescind { get; set; }

        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = string.Empty;
            }
        }

    }
}
