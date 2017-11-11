using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Digital_Content.DigitalContent.Currencies;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Digital_Content.DigitalContent.ExchangeRates
{
    public sealed class ExchangeRatesManager : DigitalContentDomainServiceBase, IExchangeRatesManager
    {
        private readonly ICurrenciesRepository _currenciesRepository;
        private readonly IRepository<ExchangeRate, long> _exchangeRepository;
        private readonly IRepository<SupportedCurrency, long> _supporteCurrenciesRepository;

        public ExchangeRatesManager(ICurrenciesRepository currenciesRepository, IRepository<SupportedCurrency, long> supporteCurrenciesRepository
            , IRepository<ExchangeRate, long> exchangeRepository)
        {
            _currenciesRepository = currenciesRepository;
            _supporteCurrenciesRepository = supporteCurrenciesRepository;
            _exchangeRepository = exchangeRepository;
        }

        public async Task<Dictionary<string, ExchangeValue>> DownloadExchangeRatesAsync()
        {
            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant))
            {
                Dictionary<string, ExchangeValue> exchangeRates = null;

                var url = "https://min-api.cryptocompare.com/data/pricemulti?fsyms={0}&tsyms=USD";

                var supportedCurrenciesQueryable = _supporteCurrenciesRepository.GetAll();

                var currenciesQueryable = _currenciesRepository.GetAll();

                var queryable = supportedCurrenciesQueryable.Join(currenciesQueryable, sc => sc.CurrencyId, c => c.Id, (sc, c) =>
                new { SupportCurrency = sc, Currency = c }).Where(o => o.Currency.IsCrypto);

                var currencies = string.Join(",", await queryable.Select(o => o.Currency.Code).ToListAsync());

                if (string.IsNullOrEmpty(currencies))
                {
                    return new Dictionary<string, ExchangeValue>();
                }

                try
                {
                    url = string.Format(url, currencies);

                    using (var httpClient = new HttpClient())
                    {
                        using (var httpResponse = await httpClient.GetAsync(url))
                        {
                            if (httpResponse.StatusCode != HttpStatusCode.OK)
                            {
                                throw new Exception($"API method: {url} returned status code: {httpResponse.StatusCode}.");
                            }

                            var jsonString = await httpResponse.Content.ReadAsStringAsync();
                            if (string.IsNullOrEmpty(jsonString))
                            {
                                throw new Exception($"API method: {url} returned empty json.");
                            }

                            exchangeRates = JsonConvert.DeserializeObject<Dictionary<string, ExchangeValue>>(jsonString);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(String.Empty, ex);
                }

                return exchangeRates ?? new Dictionary<string, ExchangeValue>();
            }
        }

        public async Task UpdateExchangeRatesAsync(Dictionary<string, ExchangeValue> exchangeRates)
        {
            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant))
            {
                try
                {
                    foreach (var item in exchangeRates)
                    {
                        if (item.Value == null)
                        {
                            continue;
                        }

                        //TODO need refactoring
                        var currency = await _currenciesRepository.FirstOrDefaultAsync(c => c.Code.Equals(item.Key));

                        if (currency == null)
                        {
                            continue;
                        }

                        ExchangeRate exchangeRate = null;

                        exchangeRate = _exchangeRepository.FirstOrDefault(c => c.CurrencyId == currency.Id);

                        if (exchangeRate == null)
                        {
                            exchangeRate = new ExchangeRate { CurrencyId = currency.Id };
                        }

                        exchangeRate.CreationTime = DateTime.Now.ToUniversalTime();
                        exchangeRate.Price = item.Value.USD;
                        exchangeRate.IsAutomaticUpdate = true;
                        exchangeRate.UpdatedBy = null;

                        await _exchangeRepository.InsertOrUpdateAsync(exchangeRate);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(String.Empty, ex);
                }
            }
        }
    }
}
