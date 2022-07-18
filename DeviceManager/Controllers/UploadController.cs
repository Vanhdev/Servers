using Aks.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DB = Aks.DB;

namespace DeviceManager
{
    class UploadController : BaseController
    {
        public static StatusAnalyzerMap Analyzers { get; set; }
        public static Device Current { get; private set; }
        public object Send(object value, string url)
        {
            //return Response(Server.ShareMemory.Name + '/' + Vst.Context.ClientId, value, 0, null, url);
            return Response(null, value, 0, null, url);
        }
        public object Send(object value)
        {
            return Send(value);
        }

        public object GetDeviceCallback(Func<Device, object> action)
        {
            var device = GetDevice();
            if (device == null) return null;

            return Response(action?.Invoke(device));
        }

        protected Device GetDevice()
        {
            return Current = DB.Devices.Find<Device>(Context.ClientId);
        }
        public object GetTime()
        {
            return Send(string.Format("TIME({0:yyyy,M,d,H,m,s})", DateTime.Now), "system_time");
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

        /// <summary>
        /// Send message to MODEL Analyzer
        /// </summary>
        /// <returns></returns>
        public object Status()
        {
            return GetDeviceCallback(device => {
                var alz = Analyzers[device];
                if (alz != null)
                {
                    var res = alz.Analyze(device, Context);
                    if (res != null)
                    {
                        History.Add(device, device.LastHistory = res);
                    }
                }
                return null;
            });
        }
        public object Alarm()
        {
            var res = Status();
            CreateSmsQueue();
            CreateCallingQueue();

            return res;
        }

        /// <summary>
        /// Message from MODEL Analyzer
        /// </summary>
        /// <returns></returns>
        public object AlarmDetected()
        {
            //var device = FindByDeviceId();

            //CreateSmsQueue(device, Context);
            //CreateCallingQueue(device, Context);

            return Ok();
        }

        /// <summary>
        /// Create SMS thread
        /// </summary>
        /// <param name="device"></param>
        /// <param name="context"></param>
        public void CreateSmsQueue() 
        { 

        }

        /// <summary>
        /// Create Calling thread
        /// </summary>
        /// <param name="device"></param>
        /// <param name="context"></param>
        public void CreateCallingQueue()
        {

        }
    }
}
