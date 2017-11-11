using System.Collections.Generic;
using System.Threading.Tasks;

namespace Digital_Content.DigitalContent.ExchangeRates
{
    public interface IExchangeRatesManager
    {
        Task<Dictionary<string, ExchangeValue>> DownloadExchangeRatesAsync();

        Task UpdateExchangeRatesAsync(Dictionary<string, ExchangeValue> exchangeRates);
    }
}
