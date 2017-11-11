using System.ComponentModel.DataAnnotations;
using Abp.Localization;

namespace Digital_Content.DigitalContent.Localization.Dto
{
    public class SetDefaultLanguageInput
    {
        [Required]
        [StringLength(ApplicationLanguage.MaxNameLength)]
        public virtual string Name { get; set; }
    }
}