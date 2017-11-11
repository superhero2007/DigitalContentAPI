using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Abp.Application.Services.Dto;
using Abp.Collections.Extensions;
using Microsoft.EntityFrameworkCore;
using Digital_Content.DigitalContent.Currencies.Dto;
using System.Globalization;
using Abp.Linq.Extensions;
using Abp.Extensions;
using Abp.Authorization;
using Digital_Content.DigitalContent.Authorization;
using Abp.UI;
using Abp.Domain.Repositories;
using Digital_Content.DigitalContent.MultiTenancy;
using Digital_Content.DigitalContent.Authorization.Users;
using Digital_Content.DigitalContent.ExchangeRates;
using Abp.Domain.Uow;

namespace Digital_Content.DigitalContent.Currencies
{
    public class CurrenciesAppService : DigitalContentAppServiceBase, ICurrenciesAppService
    {
        private readonly ICurrenciesRepository _currenciesRepository;
        private readonly IRepository<SupportedCurrency, long> _supportedCurrenciesRepository;
        private readonly IRepository<CompanyToken, int> _companyTokenRepository;
        private readonly TenantManager _tenantManager;
        private readonly IRepository<User, long> _userRepository;
        private readonly IExchangeRatesManager _exchangeManager;
        private readonly IRepository<ExchangeRate, long> _exchangeRatesRepository;
        IUnitOfWorkManager _unitOfWorkManager;

        public CurrenciesAppService(ICurrenciesRepository currenciesRepository, IRepository<SupportedCurrency, long> supportedCurrenciesRepository,
            IRepository<CompanyToken, int> companyTokenRepository,
            IRepository<User, long> userRepository, TenantManager tenantManager,
            IExchangeRatesManager exchangeManager, IRepository<ExchangeRate, long> exchangeRatesRepository,
            IUnitOfWorkManager unitOfWorkManager)
        {
            _currenciesRepository = currenciesRepository;
            _supportedCurrenciesRepository = supportedCurrenciesRepository;
            _userRepository = userRepository;
            _tenantManager = tenantManager;
            _exchangeManager = exchangeManager;
            _exchangeRatesRepository = exchangeRatesRepository;
            _unitOfWorkManager = unitOfWorkManager;
            _companyTokenRepository = companyTokenRepository;
        }

        //[AbpAuthorize(AppPermissions.Pages_Administration)]
        public async Task<PagedResultDto<CurrencyDto>> GetAvailableCurrencies(GetCurrenciesInput input)
        {
            var currencyType = ParametersToEnum(input.Parameters);

            var queryable = _currenciesRepository.GetAll();

            if (currencyType != CurrencyType.None)
            {
                queryable = queryable.Where(c => c.IsCrypto == (currencyType == CurrencyType.Crypto));
            }

            queryable = queryable.WhereIf(!input.Filter.IsNullOrWhiteSpace(), с => с.Name.Contains(input.Filter) || с.Code.Contains(input.Filter));

            var currenciesCount = await queryable.CountAsync();

            var currencies = await queryable.OrderBy(input.Sorting).PageBy(input).ToListAsync();

            return new PagedResultDto<CurrencyDto>(currenciesCount, ObjectMapper.Map<List<CurrencyDto>>(currencies));
        }

        //[AbpAuthorize(AppPermissions.Pages_Administration)]
        public async Task<PagedResultDto<SupportedCurrencyDto>> GetSupportedCurrencies(GetCurrenciesInput input)
        {
            //temp need remove
            var usdCurrency = await _currenciesRepository.FirstOrDefaultAsync(c => c.Code.Equals("USD"));

            var supportedCurrency = await _supportedCurrenciesRepository.FirstOrDefaultAsync(c => c.CurrencyId.Equals(usdCurrency.Id));

            if (supportedCurrency == null)
            {
                supportedCurrency = new SupportedCurrency
                {
                    CreationTime = DateTime.Now.ToUniversalTime(),
                    CurrencyId = usdCurrency.Id,
                    TenantId = AbpSession.TenantId,
                };

                await _supportedCurrenciesRepository.InsertAsync(supportedCurrency);

                var exchangeRate = await _exchangeRatesRepository.FirstOrDefaultAsync(er => er.CurrencyId.Equals(supportedCurrency.CurrencyId));

                if (exchangeRate == null)
                {
                    exchangeRate = new ExchangeRate { CurrencyId = usdCurrency.Id };
                    exchangeRate.CreationTime = DateTime.Now.ToUniversalTime();
                    exchangeRate.Price = 0.25M;
                    exchangeRate.IsAutomaticUpdate = true;
                    exchangeRate.UpdatedBy = null;

                    await _exchangeRatesRepository.InsertAndGetIdAsync(exchangeRate);
                }
            }
            ////

            var currencyType = ParametersToEnum(input.Parameters);

            var supportedCurrenciesQueryable = _supportedCurrenciesRepository.GetAll();

            var currenciesQueryable = _currenciesRepository.GetAll();

            var queryable = supportedCurrenciesQueryable.Join(currenciesQueryable, sc => sc.CurrencyId, c => c.Id, (sc, c) =>
            new { SupportCurrency = sc, Currency = c });

            if (currencyType != CurrencyType.None)
            {
                queryable = queryable.Where(o => o.Currency.IsCrypto == (currencyType == CurrencyType.Crypto));
            }

            queryable = queryable.WhereIf(!input.Filter.IsNullOrWhiteSpace(),
              o => o.Currency.Name.Contains(input.Filter) || o.Currency.Code.Contains(input.Filter));

            var currencyShortQueryable = queryable.Select(o =>
            new SupportedCurrencyDto(o.Currency.Id, o.Currency.Name, o.Currency.Code,
            o.Currency.ImageUrl, o.SupportCurrency.WalletId, o.SupportCurrency.WiredInstruction, o.SupportCurrency.SortOrder));

            var currenciesCount = await currencyShortQueryable.CountAsync();

            var currencies = await currencyShortQueryable.OrderBy(input.Sorting).PageBy(input).ToListAsync();

            return new PagedResultDto<SupportedCurrencyDto>(currenciesCount, currencies);
        }

        [AbpAuthorize(AppPermissions.Pages_Administration)]
        public async Task UpdateSupportedCurrencies(List<UpdateSupportCurrencyInput> input)
        {
            foreach (var supportedCurrencyDto in input)
            {
                var supportedCurrency = _supportedCurrenciesRepository.FirstOrDefault(sc => sc.CurrencyId.Equals(supportedCurrencyDto.Id));

                if (supportedCurrency == null)
                {
                    throw new UserFriendlyException(L("SupportedCurrencyNotFound"));
                }

                if (!string.IsNullOrEmpty(supportedCurrencyDto.WalletId))
                {
                    supportedCurrency.WalletId = supportedCurrencyDto.WalletId;
                }

                if (!string.IsNullOrEmpty(supportedCurrencyDto.WiredInstruction))
                {
                    supportedCurrency.WiredInstruction = supportedCurrencyDto.WiredInstruction;
                }

                if (supportedCurrencyDto.SortOrder != null)
                {
                    supportedCurrency.SortOrder = supportedCurrencyDto.SortOrder.Value;
                }

                await _supportedCurrenciesRepository.UpdateAsync(supportedCurrency);
            }
        }


        //[AbpAuthorize(AppPermissions.Pages_Administration)]
        public async Task AddSupportedCurrency(AddSupportCurrencyInput input)
        {
            var currency = _currenciesRepository.FirstOrDefault(input.Id);

            if (currency == null)
            {
                throw new UserFriendlyException(L("CurrencyNotFound"));
            }

            SupportedCurrency supportedCurrency = null;

            supportedCurrency = _supportedCurrenciesRepository.FirstOrDefault(sc => sc.CurrencyId.Equals(currency.Id));

            if (supportedCurrency != null)
            {
                throw new UserFriendlyException(L("SupportedCurrencyAlreadyExist"));
            }

            supportedCurrency = new SupportedCurrency
            {
                CreationTime = DateTime.Now.ToUniversalTime(),
                CurrencyId = input.Id,
                WalletId = input.WalletId,
                WiredInstruction = input.WiredInstruction,
                TenantId = AbpSession.TenantId,
            };

            await _supportedCurrenciesRepository.InsertAndGetIdAsync(supportedCurrency);
            var exchangeRate = await _exchangeRatesRepository.FirstOrDefaultAsync(er => er.CurrencyId.Equals(supportedCurrency.CurrencyId));

            if (exchangeRate == null)
            {
                var exchangeRates = await _exchangeManager.DownloadExchangeRatesAsync();

                await _exchangeManager.UpdateExchangeRatesAsync(exchangeRates);
            }
        }

        //[AbpAuthorize(AppPermissions.Pages_Administration)]
        public async Task RemoveSupportedCurrency(Guid id)
        {
            var currency = _currenciesRepository.FirstOrDefault(id);

            if (currency == null)
            {
                throw new UserFriendlyException(L("CurrencyNotFound"));
            }

            //tmp
            if (currency.Code.Equals("USD"))
            {
                throw new UserFriendlyException("You can not delete USD currency.");
            }

            await _supportedCurrenciesRepository.DeleteAsync(sc => sc.CurrencyId.Equals(id));
        }

        [AbpAuthorize(AppPermissions.Pages_Administration)]
        public async Task SaveICO(ICOInput input)
        {
            try
            {
                var user = await _userRepository.FirstOrDefaultAsync(item => item.Id == AbpSession.UserId);
                if (user != null)
                {
                    user.CompanyTenant = input.CompanyTenant;
                    user.TokenDescription = input.TokenDescription;
                    user.TokenName = input.TokenName;
                    user.WiredInstruction = input.WiredInstruction;
                    await _userRepository.UpdateAsync(user);
                }
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        [AbpAuthorize(AppPermissions.Pages_Administration)]
        public async Task<ICOInput> GetICO()
        {
            try
            {
                var user = await _userRepository.FirstOrDefaultAsync(item => item.Id == AbpSession.UserId);
                if (user != null)
                {
                    var outputICO = new ICOInput()
                    {
                        CompanyTenant = user.CompanyTenant,
                        TokenDescription = user.TokenDescription,
                        TokenName = user.TokenName,
                        WiredInstruction = user.WiredInstruction
                    };
                    return outputICO;
                }
                throw new Exception();
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        //ToDo:Delete method below
        public async Task<CurrentCompanyTokenDto> GetCurrentCompanyToken()
        {
            var tenantId = AbpSession.TenantId;
            if (tenantId.HasValue)
            {
                var currentCompanyToken = await _companyTokenRepository.FirstOrDefaultAsync(x => x.TenantId == tenantId);
                if (currentCompanyToken != null)
                {
                    return new CurrentCompanyTokenDto() { TokenName = currentCompanyToken.TokenName, TokenPrice = currentCompanyToken.TokenPrice };
                }
            }
            return new CurrentCompanyTokenDto() { TokenName = "DCNT", TokenPrice = 0.25M };
        }


        private CurrencyType ParametersToEnum(string parameters)
        {
            if (string.IsNullOrEmpty(parameters))
            {
                return CurrencyType.None;
            }

            foreach (var currencyType in parameters.ToLower(CultureInfo.InvariantCulture).Split(','))
            {
                switch (currencyType.Trim())
                {
                    case CurrenciesConstants.FIAT:
                        return CurrencyType.Fiat;

                    case CurrenciesConstants.CRYPTO:
                        return CurrencyType.Crypto;

                    default:
                        return CurrencyType.None;
                }
            }

            return CurrencyType.None;
        }

        private enum CurrencyType
        {
            None,
            Fiat,
            Crypto,
        }

        private class CurrenciesConstants
        {
            public const string FIAT = "fiat";
            public const string CRYPTO = "crypto";
        }
    }
}
