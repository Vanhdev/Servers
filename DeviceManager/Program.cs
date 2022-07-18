using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManager
{
    using DB = Aks.DB;
    class Program : Vst.Server.SlaveServer
    {
        protected override void RaiseStarting(string path, string name)
        {
            base.RaiseStarting(path, "device");

            DB.Main = this.CreateDefaultDB();
            
            var configs = DB.DeviceVersions.LoadConfigFolder(path + "/Config");
            UploadController.Analyzers = Aks.Devices.StatusAnalyzer.CreateAnalyzers(path, configs);

            Screen.Info(string.Format($"{DB.Devices.Count} device(s) in system."));
        }
        static void Main(string[] args)
        {
            var engine = new Program();
            engine.Start();
        }
    }
}
