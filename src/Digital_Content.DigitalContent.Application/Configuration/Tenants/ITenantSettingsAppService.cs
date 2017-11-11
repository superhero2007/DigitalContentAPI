using System.Threading.Tasks;
using Abp.Application.Services;
using Digital_Content.DigitalContent.Configuration.Tenants.Dto;
using Digital_Content.DigitalContent.Configuration.Host.Dto;

namespace Digital_Content.DigitalContent.Configuration.Tenants
{
    public interface ITenantSettingsAppService : IApplicationService
    {
        Task<TenantSettingsEditDto> GetAllSettings();

        Task UpdateAllSettings(TenantSettingsEditDto input);

        Task UpdateSmtpClientAsync(EmailSettingsEditDto input);

        Task ClearLogo();

        Task ClearCustomCss();
    }
}
