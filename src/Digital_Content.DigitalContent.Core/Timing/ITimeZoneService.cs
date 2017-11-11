using System.Threading.Tasks;
using Abp.Configuration;

namespace Digital_Content.DigitalContent.Timing
{
    public interface ITimeZoneService
    {
        Task<string> GetDefaultTimezoneAsync(SettingScopes scope, int? tenantId);
    }
}
