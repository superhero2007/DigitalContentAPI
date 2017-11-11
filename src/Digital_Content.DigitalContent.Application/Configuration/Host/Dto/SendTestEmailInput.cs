using System.ComponentModel.DataAnnotations;
using Digital_Content.DigitalContent.Authorization.Users;

namespace Digital_Content.DigitalContent.Configuration.Host.Dto
{
    public class SendTestEmailInput
    {
        [Required]
        [MaxLength(User.MaxEmailAddressLength)]
        public string EmailAddress { get; set; }
    }
}