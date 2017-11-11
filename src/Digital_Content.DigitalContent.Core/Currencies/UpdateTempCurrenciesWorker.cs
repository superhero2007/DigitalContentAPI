using System;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.Threading.Timers;
using System.Collections.Generic;
using Abp.Threading.BackgroundWorkers;
using Digital_Content.DigitalContent.ExchangeRates;
using Abp.Domain.Repositories;

namespace Digital_Content.DigitalContent.Currencies
{
    public class UpdateTempCurrenciesWorker : PeriodicBackgroundWorkerBase, ISingletonDependency
    {
        private readonly ICurrenciesRepository _currenciesRepository;
        private readonly IRepository<SupportedCurrency, long> _supportedCurrenciesRepository;

        public UpdateTempCurrenciesWorker(AbpTimer timer, ICurrenciesRepository currenciesRepository, IRepository<SupportedCurrency, long> supportedCurrenciesRepository) : base(timer)
        {
            _currenciesRepository = currenciesRepository;
            _supportedCurrenciesRepository = supportedCurrenciesRepository;
            Timer.RunOnStart = true;
            Timer.Period = 60 * 1000;
        }

        protected async override void DoWork()
        {
            var usdCurrency = await _currenciesRepository.FirstOrDefaultAsync(c => c.Code.Equals("USD"));

            if (usdCurrency == null)
            {
                _currenciesRepository.Insert(new Currency
                {
                    CreationTime = DateTime.Now.ToUniversalTime(),
                    Name = "United States Dollar",
                    Code = "USD",
                    ImageUrl = "/assets/purchase-tokens/usd-icon.svg"
                });
            }

            var dcntCurrency = await _currenciesRepository.FirstOrDefaultAsync(c => c.Code.Equals("DCNT"));

            if (dcntCurrency == null)
            {
                _currenciesRepository.Insert(new Currency
                {
                    CreationTime = DateTime.Now.ToUniversalTime(),
                    Name = "DCNT",
                    Code = "DCNT",
                    ImageUrl = "/assets/purchase-tokens/dc-icon.svg"
                });
            }

            Stop();
        }
    }
}
