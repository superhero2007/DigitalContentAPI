using System.Threading.Tasks;
using Abp.Application.Services;
using Digital_Content.DigitalContent.Editions.Dto;
using Digital_Content.DigitalContent.MultiTenancy.Dto;

namespace Digital_Content.DigitalContent.MultiTenancy
{
    public interface ITenantRegistrationAppService: IApplicationService
    {
        Task<RegisterTenantOutput> RegisterTenant(RegisterTenantInput input);

        Task<EditionsSelectOutput> GetEditionsForSelect();

        Task<EditionSelectDto> GetEdition(int editionId);
    }
}