using System;
using System.Linq;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.ObjectMapping;
using Abp.Threading.Timers;
using System.Collections.Generic;
using Abp.Threading.BackgroundWorkers;
using Digital_Content.DigitalContent.ExchangeRates;
using Abp.Domain.Repositories;

namespace Digital_Content.DigitalContent.Currencies
{
    public class UpdateCryptoCurrenciesWorker : PeriodicBackgroundWorkerBase, ISingletonDependency
    {
        private readonly ICurrenciesManager _currenciesManager;
        private const int UPDATE_PERIOD_MILLISECONDS = 7 * 24 * 60 * 60 * 1000;

        public UpdateCryptoCurrenciesWorker(AbpTimer timer, ICurrenciesManager currenciesManager) : base(timer)
        {
            _currenciesManager = currenciesManager;
            Timer.RunOnStart = true;
            Timer.Period = UPDATE_PERIOD_MILLISECONDS;
        }

        [UnitOfWork]
        protected async override void DoWork()
        {
            await _currenciesManager.UpdateCryptoCurrenciesAsync();
        }
    }
}
