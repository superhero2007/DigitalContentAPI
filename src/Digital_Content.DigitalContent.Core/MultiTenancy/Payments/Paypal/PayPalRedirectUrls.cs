using Newtonsoft.Json;

namespace Digital_Content.DigitalContent.MultiTenancy.Payments.Paypal
{
    public class PayPalRedirectUrls
    {
        [JsonProperty("return_url")]
        public string ReturnUrl { get; set; }

        [JsonProperty("cancel_url")]
        public string CancelUrl { get; set; }
    }
}