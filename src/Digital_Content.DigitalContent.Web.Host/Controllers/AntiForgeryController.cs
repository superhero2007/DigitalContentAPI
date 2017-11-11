using Microsoft.AspNetCore.Antiforgery;

namespace Digital_Content.DigitalContent.Web.Controllers
{
    public class AntiForgeryController : DigitalContentControllerBase
    {
        private readonly IAntiforgery _antiforgery;

        public AntiForgeryController(IAntiforgery antiforgery)
        {
            _antiforgery = antiforgery;
        }

        public void GetToken()
        {
            _antiforgery.SetCookieTokenAndHeader(HttpContext);
        }
    }
}
