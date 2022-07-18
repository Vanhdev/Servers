using Aks.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aks;
using Vst;

namespace AksServer
{
    abstract class BaseController : Vst.Server.SlaveController
    {
    }

    abstract class BaseDeviceController : BaseController
    {
        Device _device;
        StatusAnalyzer _analyzer;

        public virtual string DeviceId
        {
            get => Context.ClientId;
            set => Context.SetString("#deviceId", value);
        }
        public Device Device
        {
            get
            {
                if (_device == null)
                {
                    _device = DB.Devices.Find<Device>(DeviceId);
                }
                return _device;
            }
        }
        public StatusAnalyzer Analyzer
        { 
            get
            {
                if (_analyzer == null)
                {
                    _analyzer = StatusAnalyzer.GetAnalyzer(Device);
                }
                return _analyzer;
            }
        }
        public override object TryExecute(Context context)
        {
            var action = GetMethod(RequestContext.ActionName);
            if (action != null)
            {
                return action.Invoke(this, new object[] { });
            }
            if (Device != null)
            {
                var res = TryExecuteCore(Device, context);
                if (res != null)
                {
                    return SendContext(res);
                }    
            }
            return null;
        }
        protected abstract Context TryExecuteCore(Device device, Context context);
    }
}
