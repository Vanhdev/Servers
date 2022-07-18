using System;
using System.Collections.Generic;
using System.Linq;
using System.Mvc;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Monitoring.Controllers
{
    internal class BaseController : System.Mvc.Controller
    {
        public virtual object Back()
        {
            return GoFirst();
        }
        public object GoFirst()
        {
            Engine.Execute(this.ControllerName);
            return null;
        }
        public object Error(string errorCode)
        {
            return View("Views." + this.ControllerName + ".Error", errorCode);
        }
        protected object FormView(object model)
        {
            return View(model);
        }

        protected override ActionResult View(IView view, object model)
        {
            ViewData["controller"] = RequestContext.ControllerName;
            return base.View(view, model);
        }
    }
}
