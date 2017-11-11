using Microsoft.AspNetCore.Mvc;
using Digital_Content.DigitalContent.Web.Controllers;

namespace Digital_Content.DigitalContent.Web.Public.Controllers
{
    public class AboutController : DigitalContentControllerBase
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}