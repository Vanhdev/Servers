using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManager
{
    class BaseController : Vst.Server.SlaveController
    {
        public string DeviceId
        {
            get => Context.GetString("#deviceId");
            set => Context.SetString("#deviceId", value);
        }
    }
}
