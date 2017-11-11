using Abp.Auditing;
using Digital_Content.DigitalContent.WorkflowEngine.src;
using Microsoft.AspNetCore.Mvc;
using OptimaJet.Workflow;
using OptimaJet.Workflow.Core.Runtime;
using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;

using System.Threading.Tasks;

namespace Digital_Content.DigitalContent.Web.Controllers
{
    //[Route("api/[controller]/[action]")]
    public class DesignerController : DigitalContentControllerBase
    {
        public IActionResult Index()
        {
            return View();
        }

        [DisableAuditing]
        public ActionResult API()
        {
            Stream filestream = null;
            ////if (Request.Form.Files.Count > 0)
            //    filestream = Request.Body;

            var pars = new NameValueCollection();

            foreach (var key in Request.Query)
            {
                pars.Add(key.Key, key.Value);
            }


            if (Request.Method.Equals("POST", StringComparison.InvariantCultureIgnoreCase))
            {
                var parsKeys = pars.AllKeys;
                foreach (var key in Request.Form.Keys)
                {
                    if (!parsKeys.Contains(key))
                    {
                        foreach (var reqKey in Request.Form)
                        {
                            pars.Add(reqKey.Key, reqKey.Value);
                        }
                    }
                }
            }

            var res = getRuntime.DesignerAPI(pars, filestream, true);
            var operation = pars["operation"].ToLower();
            if (operation == "downloadscheme")
                return File(Encoding.UTF8.GetBytes(res), "text/xml", "scheme.xml");
            else if (operation == "downloadschemebpmn")
                return File(UTF8Encoding.UTF8.GetBytes(res), "text/xml", "scheme.bpmn");

            return Content(res);
        }
        private WorkflowRuntime getRuntime
        {
            get
            {
                return WorkflowInit.Runtime;
            }
        }
    }
}
