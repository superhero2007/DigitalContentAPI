using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Abp.Authorization.Users;
using Abp.Configuration;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Extensions;
using Abp.Localization;
using Abp.Net.Mail;
using Digital_Content.DigitalContent.Chat;
using Digital_Content.DigitalContent.Editions;
using Digital_Content.DigitalContent.Emailing;
using Digital_Content.DigitalContent.Localization;
using Digital_Content.DigitalContent.MultiTenancy;
using Digital_Content.DigitalContent.Dto;
using Digital_Content.DigitalContent.Wallets;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net.Mail;
using Abp.UI;

namespace Digital_Content.DigitalContent.Authorization.Users
{
    /// <summary>
    /// Used to send email to users.
    /// </summary>
    public class UserEmailer : DigitalContentServiceBase, IUserEmailer, ITransientDependency
    {
        private readonly IEmailTemplateProvider _emailTemplateProvider;
        private readonly IEmailSender _emailSender;
        private readonly IRepository<Tenant> _tenantRepository;
        private readonly ICurrentUnitOfWorkProvider _unitOfWorkProvider;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IRepository<User, long> _userRepository;
        private readonly ISettingManager _settingManager;
        private readonly EditionManager _editionManager;
        public static string SendgridApikey { get; set; }
        public static string FromName { get; set; }
        public static string FromEmail { get; set; }
        public static string EmailSubject { get; set; }



        public UserEmailer(
            IEmailTemplateProvider emailTemplateProvider,
            IEmailSender emailSender,
            IRepository<Tenant> tenantRepository,
            ICurrentUnitOfWorkProvider unitOfWorkProvider,
            IUnitOfWorkManager unitOfWorkManager,
            IRepository<User, long> userRepository,
            ISettingManager settingManager, EditionManager editionManager)
        {
            _emailTemplateProvider = emailTemplateProvider;
            _emailSender = emailSender;
            _tenantRepository = tenantRepository;
            _unitOfWorkProvider = unitOfWorkProvider;
            _unitOfWorkManager = unitOfWorkManager;
            _userRepository = userRepository;
            _settingManager = settingManager;
            _editionManager = editionManager;
        }

        /// <summary>
        /// Send email activation link to user's email address.
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="link">Email activation link</param>
        /// <param name="plainPassword">
        /// Can be set to user's plain password to include it in the email.
        /// </param>
        [UnitOfWork]
        public virtual async Task SendEmailActivationLinkAsync(User user, string link, string plainPassword = null)
        {
            if (user.EmailConfirmationCode.IsNullOrEmpty())
            {
                throw new Exception("EmailConfirmationCode should be set in order to send email activation link.");
            }

            link = link.Replace("{userId}", user.Id.ToString());
            link = link.Replace("{confirmationCode}", Uri.EscapeDataString(user.EmailConfirmationCode));

            if (user.TenantId.HasValue)
            {
                link = link.Replace("{tenantId}", user.TenantId.ToString());
            }

            var tenancyName = GetTenancyNameOrNull(user.TenantId);
            var emailTemplate = GetTitleAndSubTitle(user.TenantId, L("EmailActivation_Title"), L("EmailActivation_SubTitle"));
            var mailMessage = new StringBuilder();
            emailTemplate.Replace("{EMAIL_TITLE_BROWSER}", tenancyName);
            mailMessage.AppendLine("<b>" + "Full Name:" + "</b> " + user.FullName + "<br />");

            if (!tenancyName.IsNullOrEmpty())
            {
                mailMessage.AppendLine("<b>" + "Account Created on: " + "</b> " + tenancyName + "<br />");
            }

            //mailMessage.AppendLine("<b>" + L("UserName") + "</b>: " + user.UserName + "<br />");

            if (!plainPassword.IsNullOrEmpty())
            {
                mailMessage.AppendLine("<b>" + L("Password") + "</b>: " + plainPassword + "<br />");
            }

            mailMessage.AppendLine("<br />");
            mailMessage.AppendLine(L("EmailActivation_ClickTheLinkBelowToVerifyYourEmail") + "<br /><br />");
            mailMessage.AppendLine("<a href=\"" + link + "\">" + link + "</a>");
            emailTemplate.Replace("{EMAIL_BODY}", mailMessage.ToString());
            emailTemplate.Replace("{TENANCY_NAME}", tenancyName);
            emailTemplate.Replace("{YEAR}", DateTime.Now.Year.ToString());


            var client = new SendGridClient(SendgridApikey);
            var from = new EmailAddress(FromEmail, FromName);
            var subject = L("EmailActivation_Subject");
            var to = new EmailAddress(user.EmailAddress, user.FullName);
            var htmlContent = emailTemplate;
            var msg = MailHelper.CreateSingleEmail(from, to, subject, string.Empty, htmlContent.ToString());

            await client.SendEmailAsync(msg);
            //await ReplaceBodyAndSend(user.EmailAddress, L("EmailActivation_Subject"), emailTemplate, mailMessage);
        }

        /// <summary>
        /// Sends a password reset link to user's email.
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="link">Reset link</param>
        public async Task SendPasswordResetLinkAsync(User user, string link = null)
        {
            if (user.PasswordResetCode.IsNullOrEmpty())
            {
                throw new Exception("PasswordResetCode should be set in order to send password reset link.");
            }

            var tenancyName = GetTenancyNameOrNull(user.TenantId);
            var emailTemplate = GetTitleAndSubTitle(user.TenantId, L("PasswordResetEmail_Title"), L("PasswordResetEmail_SubTitle"));
            var mailMessage = new StringBuilder();

            mailMessage.AppendLine("<b>" + L("NameSurname") + "</b>: " + user.Name + " " + user.Surname + "<br />");

            if (!tenancyName.IsNullOrEmpty())
            {
                mailMessage.AppendLine("<b>" + L("TenancyName") + "</b>: " + tenancyName + "<br />");
            }

            mailMessage.AppendLine("<b>" + L("UserName") + "</b>: " + user.UserName + "<br />");
            mailMessage.AppendLine("<b>" + L("ResetCode") + "</b>: " + user.PasswordResetCode + "<br />");

            if (!link.IsNullOrEmpty())
            {
                link = link.Replace("{userId}", user.Id.ToString());
                link = link.Replace("{resetCode}", Uri.EscapeDataString(user.PasswordResetCode));

                if (user.TenantId.HasValue)
                {
                    link = link.Replace("{tenantId}", user.TenantId.ToString());
                }

                mailMessage.AppendLine("<br />");
                mailMessage.AppendLine(L("PasswordResetEmail_ClickTheLinkBelowToResetYourPassword") + "<br /><br />");
                mailMessage.AppendLine("<a href=\"" + link + "\">" + link + "</a>");
            }
            mailMessage.AppendLine("<br />");
            mailMessage.AppendLine(L("EmailActivation_ClickTheLinkBelowToVerifyYourEmail") + "<br /><br />");
            mailMessage.AppendLine("<a href=\"" + link + "\">" + link + "</a>");
            emailTemplate.Replace("{EMAIL_BODY}", mailMessage.ToString());
            var client = new SendGridClient(SendgridApikey);
            var from = new EmailAddress(FromEmail, FromName);
            var subject = L("EmailActivation_Subject");
            var to = new EmailAddress(user.EmailAddress, user.FullName);
            var htmlContent = emailTemplate;
            var msg = MailHelper.CreateSingleEmail(from, to, subject, string.Empty, htmlContent.ToString());

            await client.SendEmailAsync(msg);
            //await ReplaceBodyAndSend(user.EmailAddress, L("PasswordResetEmail_Subject"), emailTemplate, mailMessage);
        }

        public async void TryToSendChatMessageMail(User user, string senderUsername, string senderTenancyName, ChatMessage chatMessage)
        {
            try
            {
                var emailTemplate = GetTitleAndSubTitle(user.TenantId, L("NewChatMessageEmail_Title"), L("NewChatMessageEmail_SubTitle"));
                var mailMessage = new StringBuilder();

                mailMessage.AppendLine("<b>" + L("Sender") + "</b>: " + senderTenancyName + "/" + senderUsername + "<br />");
                mailMessage.AppendLine("<b>" + L("Time") + "</b>: " + chatMessage.CreationTime.ToString("yyyy-MM-dd HH:mm:ss") + "<br />");
                mailMessage.AppendLine("<b>" + L("Message") + "</b>: " + chatMessage.Message + "<br />");
                mailMessage.AppendLine("<br />");

                await ReplaceBodyAndSend(user.EmailAddress, L("NewChatMessageEmail_Subject"), emailTemplate, mailMessage);
            }
            catch (Exception exception)
            {
                Logger.Error(exception.Message, exception);
            }
        }

        public async void TryToSendSubscriptionExpireEmail(int tenantId, DateTime utcNow)
        {
            try
            {
                using (_unitOfWorkManager.Begin())
                {
                    using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                    {
                        var tenantAdmin = _userRepository.FirstOrDefault(u => u.UserName == AbpUserBase.AdminUserName);
                        if (tenantAdmin == null || string.IsNullOrEmpty(tenantAdmin.EmailAddress))
                        {
                            return;
                        }

                        var hostAdminLanguage = _settingManager.GetSettingValueForUser(LocalizationSettingNames.DefaultLanguage, tenantAdmin.TenantId, tenantAdmin.Id);
                        var culture = CultureHelper.GetCultureInfoByChecking(hostAdminLanguage);
                        var emailTemplate = GetTitleAndSubTitle(tenantId, L("SubscriptionExpire_Title"), L("SubscriptionExpire_SubTitle"));
                        var mailMessage = new StringBuilder();

                        mailMessage.AppendLine("<b>" + L("Message") + "</b>: " + L("SubscriptionExpire_Email_Body", culture, utcNow.ToString("yyyy-MM-dd") + " UTC") + "<br />");
                        mailMessage.AppendLine("<br />");

                        await ReplaceBodyAndSend(tenantAdmin.EmailAddress, L("SubscriptionExpire_Email_Subject"), emailTemplate, mailMessage);
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception.Message, exception);
            }
        }

        public async Task TryToSendSubscriptionAssignedToAnotherEmail(int tenantId, DateTime utcNow, int expiringEditionId)
        {
            try
            {
                using (_unitOfWorkManager.Begin())
                {
                    using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                    {
                        var tenantAdmin = _userRepository.FirstOrDefault(u => u.UserName == AbpUserBase.AdminUserName);
                        if (tenantAdmin == null || string.IsNullOrEmpty(tenantAdmin.EmailAddress))
                        {
                            return;
                        }

                        var hostAdminLanguage = _settingManager.GetSettingValueForUser(LocalizationSettingNames.DefaultLanguage, tenantAdmin.TenantId, tenantAdmin.Id);
                        var culture = CultureHelper.GetCultureInfoByChecking(hostAdminLanguage);
                        var expringEdition = await _editionManager.GetByIdAsync(expiringEditionId);
                        var emailTemplate = GetTitleAndSubTitle(tenantId, L("SubscriptionExpire_Title"), L("SubscriptionExpire_SubTitle"));
                        var mailMessage = new StringBuilder();

                        mailMessage.AppendLine("<b>" + L("Message") + "</b>: " + L("SubscriptionAssignedToAnother_Email_Body", culture, expringEdition.DisplayName, utcNow.ToString("yyyy-MM-dd") + " UTC") + "<br />");
                        mailMessage.AppendLine("<br />");

                        await ReplaceBodyAndSend(tenantAdmin.EmailAddress, L("SubscriptionExpire_Email_Subject"), emailTemplate, mailMessage);
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception.Message, exception);
            }
        }

        public async void TryToSendFailedSubscriptionTerminationsEmail(List<string> failedTenancyNames, DateTime utcNow)
        {
            try
            {
                var hostAdmin = _userRepository.FirstOrDefault(u => u.UserName == AbpUserBase.AdminUserName);
                if (hostAdmin == null || string.IsNullOrEmpty(hostAdmin.EmailAddress))
                {
                    return;
                }

                var hostAdminLanguage = _settingManager.GetSettingValueForUser(LocalizationSettingNames.DefaultLanguage, hostAdmin.TenantId, hostAdmin.Id);
                var culture = CultureHelper.GetCultureInfoByChecking(hostAdminLanguage);
                var emailTemplate = GetTitleAndSubTitle(null, L("FailedSubscriptionTerminations_Title"), L("FailedSubscriptionTerminations_SubTitle"));
                var mailMessage = new StringBuilder();

                mailMessage.AppendLine("<b>" + L("Message") + "</b>: " + L("FailedSubscriptionTerminations_Email_Body", culture, string.Join(",", failedTenancyNames), utcNow.ToString("yyyy-MM-dd") + " UTC") + "<br />");
                mailMessage.AppendLine("<br />");

                await ReplaceBodyAndSend(hostAdmin.EmailAddress, L("FailedSubscriptionTerminations_Email_Subject"), emailTemplate, mailMessage);
            }
            catch (Exception exception)
            {
                Logger.Error(exception.Message, exception);
            }
        }

        public async void TryToSendSubscriptionExpiringSoonEmail(int tenantId, DateTime dateToCheckRemainingDayCount)
        {
            try
            {
                using (_unitOfWorkManager.Begin())
                {
                    using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                    {
                        var tenantAdmin = _userRepository.FirstOrDefault(u => u.UserName == AbpUserBase.AdminUserName);
                        if (tenantAdmin == null || string.IsNullOrEmpty(tenantAdmin.EmailAddress))
                        {
                            return;
                        }

                        var tenantAdminLanguage = _settingManager.GetSettingValueForUser(LocalizationSettingNames.DefaultLanguage, tenantAdmin.TenantId, tenantAdmin.Id);
                        var culture = CultureHelper.GetCultureInfoByChecking(tenantAdminLanguage);

                        var emailTemplate = GetTitleAndSubTitle(null, L("SubscriptionExpiringSoon_Title"), L("SubscriptionExpiringSoon_SubTitle"));
                        var mailMessage = new StringBuilder();

                        mailMessage.AppendLine("<b>" + L("Message") + "</b>: " + L("SubscriptionExpiringSoon_Email_Body", culture, dateToCheckRemainingDayCount.ToString("yyyy-MM-dd") + " UTC") + "<br />");
                        mailMessage.AppendLine("<br />");

                        await ReplaceBodyAndSend(tenantAdmin.EmailAddress, L("SubscriptionExpiringSoon_Email_Subject"), emailTemplate, mailMessage);
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception.Message, exception);
            }
        }

        private string GetTenancyNameOrNull(int? tenantId)
        {
            if (tenantId == null)
            {
                return null;
            }

            using (_unitOfWorkProvider.Current.SetTenantId(null))
            {
                return _tenantRepository.Get(tenantId.Value).TenancyName;
            }
        }

        private StringBuilder GetTitleAndSubTitle(int? tenantId, string title, string subTitle)
        {
            var emailTemplate = new StringBuilder(_emailTemplateProvider.GetDefaultTemplate(tenantId));
            emailTemplate.Replace("{EMAIL_TITLE}", title);
            emailTemplate.Replace("{EMAIL_SUB_TITLE}", subTitle);

            return emailTemplate;
        }

        private async Task ReplaceBodyAndSend(string emailAddress, string subject, StringBuilder emailTemplate, StringBuilder mailMessage)
        {
           
            await _emailSender.SendAsync(emailAddress, subject, emailTemplate.ToString());
        }


        public string GetDCEmailTemplate(DcntEmailTemplateInput data)
        {
            //TODO email
            var address = data.WalletId;
            var body = new StringBuilder();
            body.AppendLine("<table border = '0' cellpadding = '0' cellspacing = '0' style = 'max-width:800px; margin:auto; padding:0;font-family: Tahoma, Geneva, sans-serif;' width = '100%'>");
            body.AppendLine("<tr style = 'background:#313F4D'>");
            body.AppendLine("<td colspan='3'><img style='margin-left:10px' height='50px' src='" + data.Uri + "'></td>");
            body.AppendLine("</tr>");
            body.AppendLine("<tr>");
            body.AppendLine("<td colspan = '10' style = 'background:#EEEEEE; color:white;padding: 50px'>");
            body.AppendLine("<h1 style='color:#292b2c'> Dear " + data.UserName + ", </h1> ");
            body.AppendLine("<p style='font-size:20px;color:#292b2c;'>Thank you for purchasing <i style='color:#292b2c'>" + data.Amount + " SLVT </i> tokens valued at <i style='color:#292b2c'>$ " + data.Dollar + ".</i>  Please send <i style='color:#292b2c'>" + data.DcntInBtcOrEthOrUsd.ToString("0.00000000") + " " + data.CurrentType.ToUpper() + " </i> to wallet address: </p>");
            body.AppendLine("<p align ='center' style='font-size:20px;color:#292b2c;'><i style='color:#292b2c'>" + address + " </i> </p>");
            body.AppendLine("<img style='display: block; margin: auto' src='" + data.QrCode + "' alt='' width='190' height='180' sizes ='(max-width:270px) 100vw, 270px'>");
            body.AppendLine("<p style='font-size:20px;color:#292b2c;'>Thank you,</p>");
            body.AppendLine("<p style='font-size:20px;color:#292b2c;'>The Silvertoken Team</p>");
            body.AppendLine("<p style='font-size:20px;color:#292b2c;'>Silvertoken.com</p>");
            body.AppendLine("</td>");
            body.AppendLine("</tr>");
            body.AppendLine("<tr>");
            body.AppendLine("<td align = 'center' style = 'background: #313F4D' height = '60'>");
            body.AppendLine("<span style = 'color:white; margin: 20px 0; ' > Copyright | Silvertoken | 2017 </span>");
            body.AppendLine("</td>");
            body.AppendLine("</tr>");
            body.AppendLine("</table>");

            return body.ToString();
        }

        public async Task SendEmailWithSendGrid(MailAddress receiver, string htmlBody)
        {
            try
            {
                var client = new SendGridClient(SendgridApikey);
                var from = new EmailAddress(FromEmail, FromName);
                var subject = EmailSubject;
                var to = new EmailAddress(receiver.Address, receiver.DisplayName);
                var htmlContent = htmlBody;
                var msg = MailHelper.CreateSingleEmail(from, to, subject, string.Empty, htmlContent);
                
                var response = await client.SendEmailAsync(msg);

            }
            catch (Exception ex)
            {
                throw new UserFriendlyException();
            }

        }
    }
}