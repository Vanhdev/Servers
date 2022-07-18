using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Vst;

namespace System
{
}


namespace Aks.Devices
{
    public partial class DeviceStatus : DataContext
    {
        public void AddSignal(string name)
        {
            this.Add(name, 1);
        }
        public void Read(string key, Bitwise s, int length)
        {
            for (int i = 0; i < length; i++)
            {
                if (s[i]) { this.AddSignal(key + i); }
            }
        }
    }

    public abstract class StatusAnalyzer
    {
        static StatusAnalyzerMap _map;
        static public StatusAnalyzer GetAnalyzer(string model, string version)
        {
            return _map[model + '.' + version];
        }
        static public StatusAnalyzer GetAnalyzer(Device device)
        {
            return GetAnalyzer(device.Model, device.Version);
        }

        static public void CreateAnalyzers(string path, DeviceConfigCollection configs)
        {
            var map = new Dictionary<string, Assembly>();
            var ver = new StatusAnalyzerMap();

            foreach (var item in DB.DeviceVersions)
            {
                if (map.ContainsKey(item.Model)) { continue; }

                try
                {
                    var assembly = Assembly.LoadFile(path + '\\' + item.Model + "Model.dll");
                    map.Add(item.Model, assembly);
                    if (assembly == null) { continue; }

                    foreach (var type in assembly.GetExportedTypes())
                    {
                        var Analyzer = Activator.CreateInstance(type) as StatusAnalyzer;
                        if (Analyzer != null)
                        {
                            ver.Add(item.Model + '.' + type.Name, Analyzer);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            _map = ver;
        }

        public abstract Context ProcessDeviceMessage(string actionName, Device device, Context context);
        public abstract Context ProcessUserMessage(string actionName, Device device, Context context);
        public abstract Context ProcessRemoteSetting(string actionName, Device device, Context context);
        //public abstract Context ProcessRemoteRequest(Device device, Context context);
    }

    public class StatusAnalyzerMap : BsonData.BsonDataMap<StatusAnalyzer>
    {
    }

}