using System;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace Digital_Content.DigitalContent.Currencies
{
    public class CryptoCurrency
    {
        public long Id { get; set; }

        public string Url { get; set; }

        private string imageUrl;//TODO: temporary code
        public string ImageUrl
        {
            get
            {
                if (string.IsNullOrEmpty(imageUrl) || imageUrl.Contains("https://www.cryptocompare.com"))
                {
                    return imageUrl;
                }

                return string.Format("https://www.cryptocompare.com{0}", imageUrl);
            }
            set
            {
                imageUrl = value;
            }
        }

        [JsonProperty("CoinName")]
        public string Name { get; set; }

        [JsonProperty("Symbol")]
        public string Code { get; set; }

        public string FullName { get; set; }

        public string Algorithm { get; set; }

        public string ProofType { get; set; }

        public string FullyPremined { get; set; }

        public string TotalCoinSupply { get; set; }

        public string PreMinedValue { get; set; }

        public string TotalCoinsFreeFloat { get; set; }

        public int? SortOrder { get; set; }

        public bool? Sponsored { get; set; }
    }
}
