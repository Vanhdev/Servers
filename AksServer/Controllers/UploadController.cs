using Aks.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DB = Aks.DB;

namespace AksServer.Controllers
{
    partial class UploadController
    {
        public object Send(object value, string url)
        {
            return Response(null, value, 0, null, url);
        }
        public object Send(object value)
        {
            return Send(value);
        }

        ///// <summary>
        ///// Send message to MODEL Analyzer
        ///// </summary>
        ///// <returns></returns>
        //public object Status()
        //{
        //    return GetDeviceCallback(device => {
        //        var alz = Analyzers[device];
        //        if (alz != null)
        //        {
        //            var res = alz.Analyze(device, Context);
        //            if (res != null)
        //            {
        //                History.Add(device, device.LastHistory = res);
        //            }
        //        }
        //        return null;
        //    });
        //}
        //public object Alarm()
        //{
        //    var res = Status();
        //    CreateSmsQueue();
        //    CreateCallingQueue();

        //    return res;
        //}

        ///// <summary>
        ///// Message from MODEL Analyzer
        ///// </summary>
        ///// <returns></returns>
        //public object AlarmDetected()
        //{
        //    //var device = FindByDeviceId();

        //    //CreateSmsQueue(device, Context);
        //    //CreateCallingQueue(device, Context);

        //    return Ok();
        //}

        ///// <summary>
        ///// Create SMS thread
        ///// </summary>
        ///// <param name="device"></param>
        ///// <param name="context"></param>
        //public void CreateSmsQueue() 
        //{ 

        //}

        ///// <summary>
        ///// Create Calling thread
        ///// </summary>
        ///// <param name="device"></param>
        ///// <param name="context"></param>
        //public void CreateCallingQueue()
        //{

        //}
    }
}
