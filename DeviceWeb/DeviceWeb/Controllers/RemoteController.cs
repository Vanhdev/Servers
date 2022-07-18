using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vst;

namespace DeviceWeb.Controllers
{
    public class RemoteController : BaseController
    {
        Models.Device GetDevice(string id)
        {
            var device = DeviceList[id];
            if (device.Config == null)
            {
                User.DeviceId = id;

                var context = CallService("remote/getDeviceConfig");
                device.Config = context.GetObject<Aks.Devices.DeviceConfig>("value");
            }
            return device;
        }
        public ActionResult Index(string id)
        {
            return TokenView(() => GetDevice(id).ToBase64());
        }
        public ActionResult Setting(string id)
        {
            return TokenView(() => GetDevice(id).ToBase64());
        }
        public ActionResult History(string id)
        {
            return TokenView(() => {
                this.User.Push("#deviceId", id);
                var his = DataContext.FromObject(CallService("remote/getHistory")["value"]);
                his.Remove("_id");

                return his;
            });
        }
    }
}