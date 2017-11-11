using Microsoft.AspNetCore.Mvc;
using Digital_Content.DigitalContent.Web.Controllers;

namespace Digital_Content.DigitalContent.Web.Public.Controllers
{
    public class HomeController : DigitalContentControllerBase
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}