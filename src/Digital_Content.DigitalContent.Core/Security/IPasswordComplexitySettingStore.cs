using System.Threading.Tasks;

namespace Digital_Content.DigitalContent.Security
{
    public interface IPasswordComplexitySettingStore
    {
        Task<PasswordComplexitySetting> GetSettingsAsync();
    }
}
