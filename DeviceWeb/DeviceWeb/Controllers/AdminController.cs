using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DeviceWeb.Controllers
{
    public class AdminController : BaseController
    {
        // GET: Admin
        public ActionResult Index()
        {
            var lst = DeviceList;
            if (lst.Count == 1)
            {
                return Redirect("/Remote/Index/" + lst[0].ObjectId);
            }
            return View(lst.ToArray());
        }
        public ActionResult Customer()
        {
            return View();
        }
    }
}