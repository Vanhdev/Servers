using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace Models
{
    public class ProcessInfo : BsonData.Document
    {
        public string Title 
        { 
            get => GetString("title") ?? Name.Words(); 
            set => Push("title", value); 
        }
        public string Name { get { return GetString("name"); } set => Push("name", value); }
        public string Path
        { 
            get => GetString("path");
            set => Push("path", value);
        }
        public string FileName 
        { 
            get => GetString("fileName") ?? Name + ".exe";
            set => Push("fileName", value); 
        }
        public int MemorySize { get { return GetValue<int>("memorySize"); } set => Push("memorySize", value); }
        public int MainThreadInterval { get { return GetValue<int>("threadInterval"); } set => Push("threadInterval", value); }

        public string FullPath
        {
            get => Path + '\\' + FileName;
            set
            {
                var i = value.LastIndexOf('\\');
                Path = value.Substring(0, i);
                FileName = value.Substring(i + 1);

                _moduleName = null;
            }
        }
        
        bool _alive;
        string _moduleName;
        public string ModuleName
        {
            get
            {
                if (_moduleName == null)
                {
                    string name = FileName;
                    var i = name.LastIndexOf('.');

                    _moduleName = name.Substring(0, i);
                }
                return _moduleName;
            }
        }
        public ProcessInfo CheckAlive()
        {
            var procs = Process.GetProcessesByName(ModuleName);
            bool running = procs.Length > 0;

            IsRunning = running;
            if (IsRunning)
            {

            }
            return this;
        }
        public bool IsRunning 
        {
            get => _alive; 
            set
            {
                if (_alive != value)
                {
                    _alive = value;
                    AliveChanged?.Invoke(this, EventArgs.Empty);
                }
                Push("State", _alive ? "Running" : "");
            }
        }
        public event EventHandler AliveChanged;
        public bool Start()
        {
            if (CheckAlive().IsRunning) { return false; }
            var info = new ProcessStartInfo {
                WindowStyle = ProcessWindowStyle.Hidden,
                FileName = FullPath,
            };
            Process.Start(info);

            IsRunning = true;
            return true;
        }
        public void Stop()
        {
            foreach (var p in Process.GetProcessesByName(ModuleName))
            {
                p.Kill();
            }
            IsRunning = false;
        }
        public ProcessInfo() 
        {
            MemorySize = 1;
            MainThreadInterval = 10;
        }
    }
}
