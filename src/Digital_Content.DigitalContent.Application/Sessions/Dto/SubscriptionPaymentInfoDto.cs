using Abp.AutoMapper;
using Digital_Content.DigitalContent.MultiTenancy.Payments;

namespace Digital_Content.DigitalContent.Sessions.Dto
{
    [AutoMapFrom(typeof(SubscriptionPayment))]
    public class SubscriptionPaymentInfoDto
    {
        public decimal Amount { get; set; }
    }
}