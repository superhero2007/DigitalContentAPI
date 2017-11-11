using Digital_Content.DigitalContent.Editions.Dto;

namespace Digital_Content.DigitalContent.MultiTenancy.Payments.Dto
{
    public class PaymentInfoDto
    {
        public EditionSelectDto Edition { get; set; }

        public decimal AdditionalPrice { get; set; }
    }
}
