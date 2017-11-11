using Abp.Configuration;
using Abp.Net.Mail;
using Abp.Net.Mail.Smtp;
using Abp.Runtime.Security;

namespace Digital_Content.DigitalContent.Emailing
{
    public class DigitalContentSmtpEmailSenderConfiguration : SmtpEmailSenderConfiguration
    {
        public DigitalContentSmtpEmailSenderConfiguration(ISettingManager settingManager) : base(settingManager)
        {

        }

        public override string Password => SimpleStringCipher.Instance.Decrypt(GetNotEmptySettingValue(EmailSettingNames.Smtp.Password));
    }
}