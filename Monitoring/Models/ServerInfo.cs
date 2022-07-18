using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BsonData;
using Newtonsoft.Json.Linq;

namespace Models
{
    public class ServerMap : Dictionary<string, ProcessInfo>
    {
        string _src;
        public ServerMap()
        {
            var path = Environment.CurrentDirectory;

            _src = path + @"\MainDB\process.json";
            using (var sr = new System.IO.StreamReader(_src))
            {
                var map = JObject.Parse(sr.ReadToEnd())
                    .ToObject<Dictionary<string, DataContext>>();
                foreach (var pair in map)
                {
                    var p = new ProcessInfo();
                    pair.Value.Move(p, pair.Value.Keys.ToArray());

                    if (p.Path == null) { p.Path = path; }
                    if (p.Name == null) { p.Name = pair.Key; }

                    this[p.Name] = p;
                }
            }
            CheckServerState();
        }

        public void CheckServerState()
        {
            foreach (var p in Values)
            {
                p.CheckAlive();
            }
        }

        public void Save()
        {
            using (var sw = new System.IO.StreamWriter(_src))
            {
                sw.Write(JObject.FromObject(this).ToString());
            }
        }
        public void Remove(IEnumerable<object> lst)
        {
            foreach (var e in lst)
            {
                base.Remove(((ProcessInfo)e).Name);
            }
            Save();
        }
        public bool Update(DataContext doc)
        {
            var fullPath = doc.Pop<string>("fullPath");
            var p = new ProcessInfo();

            p.Copy(doc);
            p.FullPath = fullPath;

            if (!this.ContainsKey(p.Name))
            {
                base.Add(p.Name, p);
            }
            else
            {
                this[p.Name] = p;
            }
            Save();
            return true;
        }
        public ProcessInfo Find(string name)
        {
            ProcessInfo p;
            TryGetValue(name.ToLower(), out p);
            return p;
        }
        public ProcessInfo Find(DataContext context)
        {
            return Find(context.GetString("Name"));
        }
        public IEnumerable<ProcessInfo> ToList()
        {
            return Values.OrderBy(e => e.Name);
        }
        public void StartAll()
        {
            new Thread(new ThreadStart(() => {
                MemoryInfo.Start();
                BrokerInfo.Start();
                foreach (var p in this.Values)
                {
                    p.Start();
                }
            })).Start();
        }
        public void StopAll()
        {
            new Thread(new ThreadStart(() => {
                foreach (var p in Values)
                {
                    p.Stop();
                }
            })).Start();
        }
        //ProcessInfo GetProc(string name)
        //{
        //    var key = name.Words();
        //    var p = this.Find(key);
        //    if (p == null)
        //    {
        //        p = new ProcessInfo { 
        //            Name = key,
        //            Path = string.Format($"{Environment.CurrentDirectory}\\{name}.exe"),
        //        };
        //        this.Add(p);
        //        DB.Servers.Insert(p);
        //    }
        //    return p;
        //}
        public ProcessInfo BrokerInfo => this["Broker"];
        public ProcessInfo MemoryInfo => this["Memory"];
    }
}
