using System.Threading.Tasks;
using Abp.Net.Mail;
using Digital_Content.DigitalContent.Configuration.Host.Dto;

namespace Digital_Content.DigitalContent.Configuration
{
    public abstract class SettingsAppServiceBase : DigitalContentAppServiceBase
    {
        private readonly IEmailSender _emailSender;

        protected SettingsAppServiceBase(
            IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        #region Send Test Email

        public async Task SendTestEmail(SendTestEmailInput input)
        {
            await _emailSender.SendAsync(
                input.EmailAddress,
                L("TestEmail_Subject"),
                L("TestEmail_Body")
            );
        }

        #endregion
    }
}
