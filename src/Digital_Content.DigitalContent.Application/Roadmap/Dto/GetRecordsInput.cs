using Abp.Runtime.Validation;
using Digital_Content.DigitalContent.Dto;

using System.Collections.Generic;

namespace Digital_Content.DigitalContent.Roadmap.Dto
{
    public class GetRecordsInput : PagedAndSortedInputDto, IShouldNormalize
    {
        public string Filter { get; set; }

        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "CreationTime";
            }
        }
    }
}