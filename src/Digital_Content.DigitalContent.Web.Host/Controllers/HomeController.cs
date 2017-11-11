using Abp.Auditing;
using Microsoft.AspNetCore.Mvc;

namespace Digital_Content.DigitalContent.Web.Controllers
{
    public class HomeController : DigitalContentControllerBase
    {
        [DisableAuditing]
        public IActionResult Index()
        {

            return Redirect("/swagger");
        }
    }
}
