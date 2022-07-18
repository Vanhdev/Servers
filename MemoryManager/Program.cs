using Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Vst.Server;
using Mem = Vst.Server.ShareMemory;

namespace MemoryManager
{
    class Program : ServerBase
    {
        static ServerMap _servers;
        protected override void RaiseStarting(string path, string name)
        {
            this.ShareMemory = (Mem)Mem.Manager.Create(1);
            base.RaiseStarting(path, ShareMemory.Manager.Name);
        }
        protected void CreateMemory(ShareMemory sm, int capacity)
        {
            sm.Create(capacity);

            if (sm.Created)
            {
                Screen.Info(string.Format("Created {0} MB for {1}.", capacity, sm.Name));
            }
            else
            {
                Screen.Warning(sm.Name + " is exists.");
            }
        }
        protected void CreateMemory(string name, int capacity)
        {
            var sm = new ShareMemory(name);
            CreateMemory(sm, capacity);
        }

        protected override void MainThread(int interval)
        {

            _servers = new ServerMap();

            CreateMemory(Mem.Monitoring, 1);
            CreateMemory(Mem.WebServer, 1);

            foreach (var e in _servers.Values)
            {
                CreateMemory(e.Name, e.MemorySize);
            }

            this.ShareMemory.AsyncReading<ProcessInfo>(interval, lst =>
            {
                foreach (var info in lst)
                {
                    var name = info.GetString("Name");
                    try
                    {
                        int capacity = info.MemorySize;
                        if (capacity == 0)
                        {
                            var src = _servers.Find(name);
                            if (src != null)
                            {
                                capacity = src.GetValue<int>("Size");
                            }
                        }

                        if (capacity == 0) { capacity = 1; }
                        CreateMemory(name, capacity);
                    }
                    catch (Exception e)
                    {
                        Screen.Error(e.Message);
                    }
                }


            });
        }

        protected override bool Wait()
        {
            //foreach (var context in Aks.InternalContext.LoadRequests())
            //{
            //    var server = context.Pop<string>("#server");
            //    if (server == null || ShareMemory.Open(server, sm => sm.WriteObject(context)) == false)
            //    {
            //        string internalFileName = context.GetString("#file");
            //        if (!string.IsNullOrEmpty(internalFileName))
            //        {
            //            using (var sw = new System.IO.StreamWriter(internalFileName))
            //            {
            //                try
            //                {
            //                    sw.Write(Aks.InternalContext.ServerNotFound);
            //                }
            //                catch (Exception ex)
            //                {
            //                    Screen.Error("Write response to intenal file: " + ex.Message);
            //                }
            //            }
            //        }
            //    }
            //}
            //Thread.Sleep(10);
            return true;
        }

        static void Main(string[] args)
        {
            Aks.InternalContext.SetTemplatePath("C:\\Sharing");
            new Program().Start();
        }
    }
}
