using System.ComponentModel.DataAnnotations;

namespace Digital_Content.DigitalContent.Authorization.Users.Dto
{
    public class ChangeUserLanguageDto
    {
        [Required]
        public string LanguageName { get; set; }
    }
}
