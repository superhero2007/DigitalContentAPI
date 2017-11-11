using System.Threading.Tasks;
using Abp.Application.Services;
using Digital_Content.DigitalContent.Configuration.Host.Dto;

namespace Digital_Content.DigitalContent.Configuration.Host
{
    public interface IHostSettingsAppService : IApplicationService
    {
        Task<HostSettingsEditDto> GetAllSettings();

        Task UpdateAllSettings(HostSettingsEditDto input);

        Task SendTestEmail(SendTestEmailInput input);
    }
}
