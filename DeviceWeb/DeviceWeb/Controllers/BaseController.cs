using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Filters;
using Vst;
using Vst.GUI;

namespace DeviceWeb.Controllers
{
    public abstract class BaseController : Controller
    {
        #region VIEWS
        protected virtual bool CheckRole => true;
        protected override void OnAuthentication(AuthenticationContext filterContext)
        {
            if (CheckRole && User == null)
            {
                filterContext.RouteData.Values["controller"] = "Login";
                filterContext.RouteData.Values["action"] = "Index";
            }
            base.OnAuthentication(filterContext);
        }
        protected ActionResult GoFirst()
        {
            return RedirectToAction("Index");
        }
        public ActionResult GoHome()
        {
            return Redirect("/" + User.Role);
        }
        protected ActionResult TokenView(Func<object> createModelCallback)
        {
            if (Token == null)
            {
                return Redirect("/login");
            }
            return View(createModelCallback?.Invoke());
        }
        #endregion

        #region USER
        protected override ViewResult View(string viewName, string masterName, object model)
        {
            var user = this.User;
            if (user != null)
            {
                ViewBag.User = user;
            }
            return base.View(viewName, masterName, model);
        }
        new public User User
        {
            get => (User)Session["user"];
            set => Session["user"] = value;
        }
        public string Token => User?.GetString("#token");
        public string ServerName => User.GetString("ServerName");

        public Vst.Context CreateTokenContext(string url)
        {
            return new Vst.Context
            {
                Url = url,
                Token = this.Token,
            };
        }
        public Vst.Context CreateTokenContext(string cname, string aname)
        {
            return CreateTokenContext(cname + '/' + aname);
        }
        #endregion

        public DataContext CallService(string url, object args)
        {
            var user = this.User;
            if (user == null)
            {
                return null;
            }
            var context = new Vst.Context();
            context.Copy(user);

            context.Value = args;
            context.Url = url;

            return new CommonController().Post(context) as DataContext;
        }
        public DataContext CallService(string url)
        {
            return CallService(url, null);
        }

        public DeviceList DeviceList
        {
            get
            {
                const string key = "device-list";
                var lst = Session[key] as DeviceList;
                if (lst == null)
                {
                    var context = CallService("service/getDeviceList");
                    if (context != null)
                    {
                        lst = context.GetArray<DeviceList>("value");
                        Session[key] = lst;
                    }
                }
                return lst ?? new DeviceList();
            }
        }
    }
}
