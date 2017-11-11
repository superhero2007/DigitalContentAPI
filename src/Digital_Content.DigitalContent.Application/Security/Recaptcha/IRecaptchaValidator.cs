using System.Threading.Tasks;

namespace Digital_Content.DigitalContent.Security.Recaptcha
{
    public interface IRecaptchaValidator
    {
        Task ValidateAsync(string captchaResponse);
    }
}