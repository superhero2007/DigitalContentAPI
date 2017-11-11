using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Digital_Content.DigitalContent.Currencies.Dto;
using System.Collections.Generic;
using System;

namespace Digital_Content.DigitalContent.Currencies
{
    public interface ICurrenciesAppService : IApplicationService
    {
        Task<PagedResultDto<CurrencyDto>> GetAvailableCurrencies(GetCurrenciesInput input);

        Task<PagedResultDto<SupportedCurrencyDto>> GetSupportedCurrencies(GetCurrenciesInput input);

        Task UpdateSupportedCurrencies(List<UpdateSupportCurrencyInput> currencies);


        Task AddSupportedCurrency(AddSupportCurrencyInput input);

        Task RemoveSupportedCurrency(Guid id);
        Task<CurrentCompanyTokenDto> GetCurrentCompanyToken();
        //Task CreateCustomCurrency(CreateCustomCurrencyInput currency);  

    }
}