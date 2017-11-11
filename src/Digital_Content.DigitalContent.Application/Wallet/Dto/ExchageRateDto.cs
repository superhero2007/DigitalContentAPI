using Digital_Content.DigitalContent.Currencies.Dto;

namespace Digital_Content.DigitalContent.Wallet.Dto
{
    public class ExchageRateDto
    {
        public long Id { get; set; }

        //public string CurrnecyType { get; set; } //TODO: old

        public CurrencyDto Currency { get; set; }

        public decimal Price { get; set; }

        public bool IsAutoWallet { get; set; }
    }
}
