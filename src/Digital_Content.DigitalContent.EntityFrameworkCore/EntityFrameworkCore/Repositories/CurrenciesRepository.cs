using System;
using System.Threading.Tasks;
using Abp.EntityFrameworkCore;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Digital_Content.DigitalContent.EntityFrameworkCore;
using Digital_Content.DigitalContent.EntityFrameworkCore.Repositories;

namespace Digital_Content.DigitalContent.Currencies
{
    public class CurrenciesRepository : DigitalContentRepositoryBase<Currency, Guid>, ICurrenciesRepository
    {
        public CurrenciesRepository(IDbContextProvider<DigitalContentDbContext> dbContextProvider) : base(dbContextProvider)
        {

        }

        public Task<int> DeleteNotSupportedCryptoCurrenciesAsync()
        {
            return Context.Database.ExecuteSqlCommandAsync($"DELETE FROM [{ nameof(Context.Currencies)}] " +
                $"WHERE [{nameof(Currency.IsCrypto)}] = 1 AND Id NOT IN(SELECT sc.[{nameof(SupportedCurrency.CurrencyId)}] " +
                $"FROM [{ nameof(Context.SupportedCurrencies)}] sc)");
        }

        public Task<int> InsertRangeAsync(IList<Currency> сurrencies)
        {
            Context.AddRange(сurrencies);
           
            var result = Context.SaveChanges();

            return Task.FromResult(result);
        }
    }
}
