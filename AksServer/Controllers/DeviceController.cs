using Aks.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vst;
using Aks;

namespace AksServer.Controllers
{
    class DeviceController : BaseDeviceController
    {
        protected override Context TryExecuteCore(Device device, Context context)
        {
            var res = Analyzer.ProcessDeviceMessage(RequestContext.ActionName, device, context);
            return res;
        }
        public object Connect()
        {
            var device = Context.ValueContext.ChangeType<Device>();
            var model = device.Model;
            var version = device.Version;

            if (model == null || version == null)
            {
                return null;
            }

            var config = DB.DeviceVersions.Find(model, version);
            device.Config = config;

            device.ObjectId = Context.ClientId;
            DB.Devices.InsertOrUpdate(device);

            return null;
        }
        public object GetTime()
        {
            //return new Mqtt.PublishContext { 
            //    Topic = Device.GetDefaultTopic(),
            //    Message = string.Format("TIME({0:yyyy,M,d,H,m,s})", DateTime.Now)
            //};

            return Response(string.Format("TIME({0:yyyy,M,d,H,m,s})", DateTime.Now));
        }
    }

    partial class RequestController: DeviceController
    {
    }

    partial class UploadController : DeviceController
    {

    }
}
