using Aks.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DB = Aks.DB;

namespace DeviceManager.Controllers
{
    class ServiceController : BaseController
    {
        public object GetDeviceInfo()
        {
            return Response(DB.Devices.Find<Device>(DeviceId));
        }
        public object GetHistory()
        {
            return Response(History.Find(DeviceId));
        }
        public object GetDeviceStatus()
        {
            var device = DB.Devices.Find<Device>(DeviceId);
            return Response(device?.LastHistory);
        }
    }
}
