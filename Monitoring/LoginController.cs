using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DeviceWeb.Controllers
{
    public class LoginController : BaseController
    {
        protected override bool CheckRole => false;
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Success(string token)
        {
            this.User = (Models.User)Models.DB.UserCollection[token];
            return GoHome();
        }
    }
}