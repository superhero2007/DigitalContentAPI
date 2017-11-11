using Abp.Application.Services;
using Digital_Content.DigitalContent.TermsOfService.Dto;
using Digital_Content.DigitalContent.TermsOfServices.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Content.DigitalContent.TermsOfServices
{
    public interface ITermsOfServicesAppService : IApplicationService
    {
        Task<bool> CreateOrUpdateTosContentAsync(TermsOfServiceInput input);

        Task<TermsOfServiceInput> GetAndAssignDefaultTosAsync();

        Task<TermsOfServiceOutput> ResetDefaultTosContent();
    }
}
