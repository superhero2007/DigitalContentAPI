using System.ComponentModel.DataAnnotations;

namespace Digital_Content.DigitalContent.Localization.Dto
{
    public class CreateOrUpdateLanguageInput
    {
        [Required]
        public ApplicationLanguageEditDto Language { get; set; }
    }
}