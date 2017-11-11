using System.Threading.Tasks;
using Abp;
using Abp.Notifications;
using Digital_Content.DigitalContent.Authorization.Users;
using Digital_Content.DigitalContent.MultiTenancy;

namespace Digital_Content.DigitalContent.Notifications
{
    public interface IAppNotifier
    {
        Task WelcomeToTheApplicationAsync(User user);

        Task NewUserRegisteredAsync(User user);

        Task NewTenantRegisteredAsync(Tenant tenant);

        Task SendMessageAsync(UserIdentifier user, string message, NotificationSeverity severity = NotificationSeverity.Info);
    }
}
