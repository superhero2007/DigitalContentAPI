using Abp.Runtime.Validation;
using Digital_Content.DigitalContent.Dto;

namespace Digital_Content.DigitalContent.MultiTenancy.Payments.Dto
{
    public class GetPaymentHistoryInput : PagedAndSortedInputDto, IShouldNormalize
    {
        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "CreationTime";
            }

            Sorting = Sorting.Replace("editionDisplayName", "Edition.DisplayName");
        }
    }
}
