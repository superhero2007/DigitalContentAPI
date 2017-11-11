using Abp.Notifications;
using Digital_Content.DigitalContent.Dto;

namespace Digital_Content.DigitalContent.Notifications.Dto
{
    public class GetUserNotificationsInput : PagedInputDto
    {
        public UserNotificationState? State { get; set; }
    }
}