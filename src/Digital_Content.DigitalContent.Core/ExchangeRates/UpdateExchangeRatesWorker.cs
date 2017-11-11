using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Threading.BackgroundWorkers;
using Abp.Threading.Timers;
using Digital_Content.DigitalContent.Currencies;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Digital_Content.DigitalContent.ExchangeRates
{
    public class UpdateExchangeRatesWorker : PeriodicBackgroundWorkerBase, ISingletonDependency
    {
        private readonly ICurrenciesRepository _currenciesRepository;
        private readonly IRepository<ExchangeRate, long> _exchangeRatesRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private const int UPDATE_PERIOD_MILLISECONDS = 5 * 60 * 1000;

        public UpdateExchangeRatesWorker(AbpTimer timer, ICurrenciesRepository currenciesRepository, IRepository<ExchangeRate, long> exchangeRatesRepository, IUnitOfWorkManager unitOfWorkManager) : base(timer)
        {
            _currenciesRepository = currenciesRepository;
            _exchangeRatesRepository = exchangeRatesRepository;
            _unitOfWorkManager = unitOfWorkManager;
            Timer.Period = UPDATE_PERIOD_MILLISECONDS;
        }

        [UnitOfWork]
        protected async override void DoWork()
        {
            try
            {
                using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant))
                {
                    var exchangeRates = _exchangeRatesRepository.GetAllList();

                    foreach (var exchangeRate in exchangeRates)
                    {
                        var currencyFrom = _currenciesRepository.FirstOrDefault(exchangeRate.CurrencyId);

                        if (currencyFrom != null)
                        {
                            await DownloadAndUpdateExchangeRateAsync(exchangeRate, currencyFrom.Code, "USD");

                            await _exchangeRatesRepository.UpdateAsync(exchangeRate);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(String.Empty, ex);
            }
        }

        [UnitOfWork]
        private async Task DownloadAndUpdateExchangeRateAsync(ExchangeRate exchangeRate, string codeFrom, string codeTo)
        {
            var url = "https://min-api.cryptocompare.com/data/price?fsym={0}&tsyms={1}";

            try
            {
                url = string.Format(url, codeFrom, codeTo);

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

                        var exchangeRates = JsonConvert.DeserializeObject<Dictionary<string, decimal>>(jsonString);

                        var result = exchangeRates.FirstOrDefault();

                        exchangeRate.CreationTime = DateTime.UtcNow;

                        exchangeRate.Price = result.Value;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(String.Empty, ex);
            }
        }
    }
}
