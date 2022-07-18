using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DeviceWeb.Controllers.API
{
    public class ProfileController : BaseController
    {
        // GET: Profile
        public ActionResult Index()
        {
            return View();
        }
    }
}