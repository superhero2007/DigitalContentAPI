using System.ComponentModel.DataAnnotations;

namespace Digital_Content.DigitalContent.Authorization.Accounts.Dto
{
    public class SendEmailActivationLinkInput
    {
        [Required]
        public string EmailAddress { get; set; }
    }
}