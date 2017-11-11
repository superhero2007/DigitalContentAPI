using Abp.Runtime.Validation;
using Digital_Content.DigitalContent.Dto;

using System.Collections.Generic;

namespace Digital_Content.DigitalContent.Currencies.Dto
{
    public class GetCurrenciesInput : PagedAndSortedInputDto, IShouldNormalize
    {
        public string Filter { get; set; }

        public string Parameters { get; set; }

        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "SortOrder";
            }
        }
    }
}