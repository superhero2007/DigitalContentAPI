using System;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using System.Collections.Generic;

namespace Digital_Content.DigitalContent.Currencies
{
    public interface ICurrenciesRepository : IRepository<Currency, Guid>
    {
        Task<int> DeleteNotSupportedCryptoCurrenciesAsync();

        Task<int> InsertRangeAsync(IList<Currency> currencies);
    }
}
