using BsonData;
using Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Monitoring
{
    partial class App
    {
        public static ServerMap ServerMap { get; set; }

    }
}

namespace Monitoring.Controllers
{
    class ServerController : BaseController
    {
        public IEnumerable<ProcessInfo> GetDataList()
        {
            if (App.ServerMap == null)
            {
                App.ServerMap = new ServerMap();
                System.Mvc.Engine.BeginInvoke(() => { 
                    while (true)
                    {
                        App.ServerMap.CheckServerState();
                        Thread.Sleep(500);
                    }
                });
            }
            return App.ServerMap.ToList();
        }
        public object Index()
        {
            return View(GetDataList());
        }
        public object Add()
        {
            return View(new ProcessInfo());
        }
        public object Details(string id)
        {
            return View(App.ServerMap[id]);
        }
        public object Update(EditingContext context)
        {
            App.ServerMap.Update(context.Value);
            return GoFirst();
        }
        public object Delete(EditingContext context)
        {
            App.ServerMap.Remove(context.Value.Values);
            return GoFirst();
        }

        public object Start(ProcessInfo p)
        {
            p.Start();
            return null;
        }
        public object Start(EditingContext context)
        {
            foreach (ProcessInfo p in context.Value.Values)
            {
                Start(p);
            }
            return GoFirst();
        }
        public object Stop(ProcessInfo p)
        {
            p.Stop();
            return null;
        }
        public object Stop(EditingContext context)
        {
            foreach (ProcessInfo p in context.Value.Values)
            {
                Stop(p);
            }
            return GoFirst();
        }
        public object Show(ProcessInfo p)
        {
            return null;
        }

        public object Broker()
        {
            return View(App.ServerMap.BrokerInfo);
        }
        public object Memory()
        {
            return View(App.ServerMap.MemoryInfo);
        }
    }
}
