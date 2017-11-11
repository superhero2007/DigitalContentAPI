using Newtonsoft.Json;


namespace Digital_Content.DigitalContent.CreditCardPayments.CreditCardDto
{
    public class PaymentData
    {
        public string Id { get; set; }

        public string Object { get; set; }

        public CreditCard Card { get; set; }

        [JsonProperty(PropertyName = "client_ip")]
        public string ClientIp { get; set; }

        public long Created { get; set; }

        public bool Livemode { get; set; }

        public string Type { get; set; }

        public bool Used { get; set; }
    }
}
