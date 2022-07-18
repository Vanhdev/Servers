using Aks.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vst;
using DB = Aks.DB;

namespace AksServer.Controllers
{
    class ServiceController : BaseDeviceController
    {
        protected override Context TryExecuteCore(Device device, Context context)
        {
            return new Context {  Code = 0 };
        }
        public override string DeviceId
        {
            get => Context.GetString("#deviceId");
            set => base.DeviceId = value;
        }
        public object GetDeviceInfo()
        {
            return Response(Device);
        }

        public object GetHistory()
        {
            return Response(History.Find(DeviceId));
        }
        public object GetDeviceStatus()
        {
            return Response(Device?.LastHistory);
        }
        public object GetDeviceList()
        {
            return Response(DB.Devices.Select());
        }
    }
    partial class RemoteController : ServiceController
    {
    }
    //partial class HistoryController : ServiceController
    //{ 
    //}
    partial class SettingController : RemoteController
    {

    }
}
