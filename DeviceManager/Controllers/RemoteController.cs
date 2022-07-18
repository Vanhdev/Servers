using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aks;
using Aks.Devices;

namespace DeviceManager
{
    class RemoteController : UploadController
    {
        protected object FindToSend(Func<Device, StatusAnalyzer, object> response)
        {
            var token = Context.Token;
            if (token == null) { return null; }

            var id = DeviceId;
            var device = DB.Devices.Find<Device>(id);
            if (device == null) return null;

            var alz = Analyzers[device];
            if (alz == null) { return null; }

            var res = response.Invoke(device, alz);
            if (res == null) { return null; }

            Context.Pop("#cid");
            Context.Push("#topic", string.Format(device.Config.Topic, id));
            Context.Push("#internal", Vst.Server.Memory.Account.Name);
            Context.Value = res;

            return Context;
        }
        public object Command()
        {
            return FindToSend((d, a) => {
                Context.Url = "device/remoteControl";
                return a.GetCommandLine(d, Context);
            });
        }

        public object Setting()
        {
            return FindToSend((d, a) => {
                Context.Url = "device/remoteSetting";
                return a.GetSettingContent(d, Context);
            });
        }
    }
}
