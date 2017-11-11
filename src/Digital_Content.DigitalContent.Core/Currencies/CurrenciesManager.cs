using System;
using System.Net;
using System.Linq;
using Abp.Domain.Uow;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Abp.Dependency;
using Abp.Domain.Repositories;

namespace Digital_Content.DigitalContent.Currencies
{
    public class CurrenciesManager : DigitalContentDomainServiceBase, ICurrenciesManager
    {
        private readonly ICurrenciesRepository _currenciesRepository;
        private readonly IRepository<SupportedCurrency, long> _supportedCurrenciesRepository;

        public CurrenciesManager(ICurrenciesRepository currenciesRepository, IRepository<SupportedCurrency, long> supportedCurrenciesRepository)
        {
            _currenciesRepository = currenciesRepository;
            _supportedCurrenciesRepository = supportedCurrenciesRepository;
        }

        [UnitOfWork]
        public async Task<IList<CryptoCurrency>> DownloadCryptoCurrenciesAsync()
        {
            List<CryptoCurrency> cryptoCurrencies = null;

            var url = "https://www.cryptocompare.com/api/data/coinlist";

            try
            {
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

                        var jsonObject = JObject.Parse(jsonString).SelectToken("Data");
                        if (jsonObject == null)
                        {
                            throw new Exception($"API method: {url} returned json where <Data> property has missed or null.");
                        }

                        cryptoCurrencies = jsonObject.ToObject<Dictionary<string, CryptoCurrency>>().Values.ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(String.Empty, ex);
            }

            return cryptoCurrencies ?? new List<CryptoCurrency>();
        }

        [UnitOfWork]
        public async Task UpdateCryptoCurrenciesAsync()
        {
            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant))
            {
                try
                {
                    var supportedCurrenciesQueryable = _supportedCurrenciesRepository.GetAll();

                    var currenciesQueryable = _currenciesRepository.GetAll();

                    var supportedCurrencies = supportedCurrenciesQueryable.Join(currenciesQueryable, sc => sc.CurrencyId, c => c.Id, (sc, c) =>
                    new { SupportCurrency = sc, Currency = c }).Where(o => o.Currency.IsCrypto).ToList();

                    var cryptoCurrencies = await DownloadCryptoCurrenciesAsync();

                    foreach (var obj in supportedCurrencies)
                    {
                        var cryptoCurrency = cryptoCurrencies.FirstOrDefault(cc => cc.Code.Equals(obj.Currency.Code));

                        if (cryptoCurrency != null)
                        {
                            cryptoCurrencies.Remove(cryptoCurrency);
                        }
                    }

                    if (cryptoCurrencies.Count == 0)//TODO: need ask about that case
                    {
                        return;
                    }

                    var currencies = cryptoCurrencies.Select(c => CurrencyMapper(c)).ToList();

                    //TODO: add symbols download

                    await _currenciesRepository.DeleteNotSupportedCryptoCurrenciesAsync();

                    await _currenciesRepository.InsertRangeAsync(currencies);
                }
                catch (Exception ex)
                {
                    Logger.Error(String.Empty, ex);
                }
            }
        }

        private Currency CurrencyMapper(CryptoCurrency cryptoCurrency)
        {
            //TODO: In the future, need to save additional information
            return new Currency
            {
                Id = Guid.NewGuid(),
                CreationTime = DateTime.Now.ToUniversalTime(),
                Name = cryptoCurrency.Name,
                Code = cryptoCurrency.Code,
                ImageUrl = cryptoCurrency.ImageUrl,
                IsCrypto = true,
                SortOrder = cryptoCurrency.SortOrder
            };
        }
    }
}
