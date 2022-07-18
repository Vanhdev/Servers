using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Aks;

namespace AutoRun
{
    class Program
    {
        static Models.ServerMap map;
        Models.ProcessInfo Pop(Models.ProcessInfo p)
        {
            map.Remove(p.Name);
            return p;
        }
        static void Main(string[] args)
        {
            const string windowPath = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";
            const string appName = "SERVER_MANAGER_AUTORUN";
            string execPath = Environment.CommandLine;

            var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(windowPath, true);
            key.SetValue(appName, execPath);

            DB.Main = new BsonData.Database("MainDB")
                .Connect(Environment.CurrentDirectory);

            map = new Models.ServerMap();

            Console.WriteLine("Starting servers ...");

            var lst = new List<Models.ProcessInfo>();
            lst.Add(map.MemoryInfo);
            lst.Add(map.BrokerInfo);
            foreach (var p in map.Values)
            {
                lst.Add(p);
            }
            lst.Add(new Models.ProcessInfo {
                Name = "Monitoring",
                Path = Environment.CurrentDirectory + "\\Monitoring.exe"
            });

            foreach (var p in lst)
            {
                p.Start();
                Console.WriteLine(p.Name + " started.");
            }
        }
    }
}
