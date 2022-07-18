using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Mvc;
using System.Diagnostics;

namespace Vst.Server
{
    public abstract class ServerBase : Engine
    {
        public string Path { get; private set; }
        public event Action<string, string> OnStarting;
        protected virtual void RaiseStarting(string path, string name)
        {
            Path = path;
            if (ShareMemory == null)
            {
                ShareMemory.Manager.WriteObject(new { Name = name });
                System.Threading.Thread.Sleep(250);

                ShareMemory = new ShareMemory(name);
            }
            OnStarting?.Invoke(path, name);

            Screen.Waiting("Starting " + name.ToUpper() + " server");
        }

        #region Shared Memory
        public ShareMemory ShareMemory { get; set; }
        #endregion

        public BsonData.Database CreateDatabase(string name, string storagePath, int msStorageInterval)
        {
            if (storagePath == null)
            {
                storagePath = Path;
            }
            Screen.Error(storagePath);

            return new BsonData.Database(name)
                .Connect(storagePath)
                .StartStorageThread(msStorageInterval);
        }

        protected abstract void MainThread(int interval);
        protected virtual bool Wait() { return true; }
        protected virtual void Start()
        {
            Register(this, result => { });
            try
            {
                string name = Process.GetCurrentProcess().MainModule.FileName;
                var i = name.LastIndexOf('\\') + 1;
                var j = name.IndexOf('.', i);

                Path = name.Substring(0, i);
                name = name.Substring(i, j - i);

                Console.Title = name.Words();

                RaiseStarting(Path, name);

                MainThread(10);

                Screen.Success("Ready");
            }
            catch (Exception e)
            {
                Screen.Error("ServerBase.Start");
                Screen.Warning(e.ToString());
            }

            while (Wait()) { }
        }
    }
}
