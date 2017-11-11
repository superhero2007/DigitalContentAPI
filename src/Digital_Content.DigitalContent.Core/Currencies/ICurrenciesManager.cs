using Abp.Domain.Services;
using Abp.Domain.Uow;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Digital_Content.DigitalContent.Currencies
{
    public interface ICurrenciesManager : IDomainService
    {
        Task<IList<CryptoCurrency>> DownloadCryptoCurrenciesAsync();

        Task UpdateCryptoCurrenciesAsync();
    }
}
