using System.Threading.Tasks;
using Digital_Content.DigitalContent.Security.Recaptcha;

namespace Digital_Content.DigitalContent.Tests.Web
{
    public class FakeRecaptchaValidator : IRecaptchaValidator
    {
        public async Task ValidateAsync(string captchaResponse)
        {
            
        }
    }
}
