using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DeviceWeb.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            var user = User;
            if (user == null)
            {
                return Redirect("/login");
            }
            return GoHome();
        }
        public ActionResult Logout()
        {
            CallService("account/logout");

            Session.Abandon();
            return GoHome();
        }
    }
}
