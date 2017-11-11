using Newtonsoft.Json;


namespace Digital_Content.DigitalContent.CreditCardPayments.CreditCardDto
{
    public class CreditCard
    {

        public string Id { get; set; }

        public string Object { get; set; }

        [JsonProperty(PropertyName = "address_city")]
        public string AddressCity { get; set; }

        [JsonProperty(PropertyName = "address_country")]
        public string AddressCountry { get; set; }

        [JsonProperty(PropertyName = "address_line1")]
        public string AddressLine1 { get; set; }

        [JsonProperty(PropertyName = "address_line1_check")]
        public string AddressLine1Check { get; set; }

        [JsonProperty(PropertyName = "address_line2")]
        public string AddressLine2 { get; set; }

        [JsonProperty(PropertyName = "address_state")]
        public string AddressState { get; set; }

        [JsonProperty(PropertyName = "address_zip")]
        public string AddressZip { get; set; }

        [JsonProperty(PropertyName = "address_zip_check")]
        public string AddressZipCheck { get; set; }

        public string Brand { get; set; }
        public string Country { get; set; }

        [JsonProperty(PropertyName = "сvc_check")]
        public string CvcCheck { get; set; }

        [JsonProperty(PropertyName = "dynamic_last4")]
        public string DynamicLast4 { get; set; }

        [JsonProperty(PropertyName = "exp_month")]
        public int ExpMonth { get; set; }

        [JsonProperty(PropertyName = "exp_year")]
        public long ExpYear { get; set; }

        public string Funding { get; set; }

        public string Last4 { get; set; }

        public object Metadata { get; set; }

        public string Name { get; set; }

        [JsonProperty(PropertyName = "tokenization_method")]
        public string TokenizationMethod { get; set; }

        public string Fingerprint { get; set; }

        public string Customer { get; set; }

        public string Type { get; set; }
    }
}
