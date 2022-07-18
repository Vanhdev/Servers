using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BsonData;
using Newtonsoft.Json.Linq;

namespace Aks.Devices
{
    //public class Device : Document
    //{
    //    public Devices.DeviceStatus Status { get; set; } = new Devices.DeviceStatus();
    //    public string LastStatus { get; set; }

    //    Devices.Version _version;
    //    public Devices.Version Version
    //    {
    //        get
    //        {
    //            if (_version == null)
    //            {
    //                var v = GetString("Version");
    //                _version = Devices.Version.GetVersion(v);
    //            }
    //            return _version;
    //        }
    //    }
    //    public void Remote(string command)
    //    {
    //        Send(Version.GetString(command));
    //    }
    //    public void Send(string message)
    //    {
    //    }
    //    public void RequestInfo()
    //    {
    //        Remote("#info");
    //    }
    //    public void SendSettingPhone(DataContext context)
    //    {
    //        var function = context.GetString("Name").ToUpper();
    //        int i = 0;
    //        foreach (var number in context.GetArray<List<string>>("Numbers"))
    //        {
    //            var s = number.Trim().ToCharArray();
    //            var msg = function + string.Format($"({i},'");
    //            if (s.Length > 0)
    //            {
    //                var k = 0;
    //                switch (s[0])
    //                {
    //                    case '0': k = 1; break;
    //                    case '+': k = 3; break;
    //                }
    //                while (k < s.Length)
    //                {
    //                    char c = s[k++];
    //                    if (char.IsDigit(c))
    //                    {
    //                        msg += c;
    //                    }
    //                }
    //            }

    //            msg += "')";
    //            Send(msg);

    //            if (++i >= 5) break;
    //        }
    //    }
    //    public bool Subscribed { get; set; }
    //}
    public class BitConfig
    {
        public string Name { get; set; }
        public int Value { get; set; }
    }
    public class StatusByte
    {
        public string Name { get; set; }
        public Dictionary<string, BitConfig> Bits { get; set; } = new Dictionary<string, BitConfig>();
    }
    public class StatusArray : List<StatusByte>
    {
        public DataContext Parse(byte[] s)
        {
            var context = new DataContext();
            if (s.Length == 0) { return context; }

            int i = 0;
            foreach (var item in this)
            {
                Bitwise b = s[i++];
                foreach (var p in item.Bits)
                {
                    var bit = p.Value;
                    if (bit.Value == b.Value || (bit.Value & b.Value) != 0)
                    {
                        context.Add(p.Key, 1);
                    }
                }

                if (i >= s.Length) break;
            }
            return context;
        }
    }
    public class RemoteAction : DataContext
    {
        public List<object> Arguments { get { return GetArray<List<object>>("args"); } set => Push("args", value); }
        public string FunctionName { get { return GetString("func"); } set => Push("func", value); }

        string _command;
        public string GetCommandLine()
        {
            if (_command != null)
            {
                return _command;
            }
            var args = this.Arguments;
            _command = FunctionName + '(';
            if (args != null)
            {
                _command += string.Join(",", args);
            }
            return _command += ')';
        }
    }
    public class RemoteActionCollection : Dictionary<string, RemoteAction>
    {
        public RemoteAction FindCommand(string key)
        {
            RemoteAction action;
            TryGetValue(key.ToUpper(), out action);

            return action;
        }
    }
    public class DeviceConfig
    {
        public string Topic { get; set; }
        public StatusArray Status { get; set; }
        public DataContext Alarm { get; set; }
        public RemoteActionCollection Control { get; set; }
        public RemoteActionCollection Setting { get; set; }
        public DataContext Extends { get; } = new DataContext();
    }
    public class DeviceVersion
    {
        public string Model { get; set; }
        public string Version { get; set; }
        public DeviceConfig Config { get; set; }
    }
    public class DeviceConfigCollection : List<DeviceVersion>
    {
        public DeviceConfig Find(string model, string version)
        {
            model = model.ToUpper();
            version = version.ToUpper();

            foreach (var item in this)
            {
                if (item.Model == model && item.Version == version)
                {
                    return item.Config;
                }
            }
            return null;
        }
        public DeviceConfigCollection LoadConfigFolder(string path)
        {
            var di = new System.IO.DirectoryInfo(path);
            foreach (var fi in di.GetFiles())
            {
                using (var sr = new System.IO.StreamReader(fi.FullName))
                {
                    var lst = JArray.Parse(sr.ReadToEnd()).ToObject<DeviceConfigCollection>();
                    this.AddRange(lst);
                }
            }
            return this;
        }
    }
    public class Device : Document
    {
        public string Name { get { return GetString("name"); } set => Push("name", value); }
        public string Model { get { return GetString("model"); } set => Push("model", value); }
        public string Version { get { return GetString("version"); } set => Push("version", value); }

        DataContext _setting;
        public DataContext Setting
        {
            get
            {
                if (_setting == null)
                {
                    _setting = GetObject<DataContext>("setting") ?? new DataContext();
                }
                return _setting;
            }
        }
        public void UpdateSetting(string name, object value)
        {
            _setting.Push(name, value);
            Push("setting", _setting);

            //if (value is IList)
            //{
            //    _setting.SetArray(name, value);
            //}
            //else
            //{
            //    _setting.SetObject(name, value);
            //}
        }

        public string GetDefaultTopic() => string.Format(Config.Topic, ObjectId);

        DeviceConfig _config;
        public DeviceConfig Config
        {
            get
            {
                if (_config == null)
                {
                    _config = DB.DeviceVersions.Find(Model, Version);
                }
                return _config;
            }
            set
            {
                _config = value;
            }
        }

        public string LastStatus { get; set; }
        public DataContext LastHistory { get; set; }
    }
}

namespace Aks
{
    partial class DB
    {
        static Collection _devices;
        static public Collection Devices
        {
            get
            {
                if (_devices == null)
                {
                    _devices = GetCollection<Aks.Devices.Device>();
                }
                return _devices;
            }
        }

        static Devices.DeviceConfigCollection _deviceVersions;
        public static Devices.DeviceConfigCollection DeviceVersions
        {
            get
            {
                if (_deviceVersions == null)
                {
                    _deviceVersions = new Devices.DeviceConfigCollection();
                }
                return _deviceVersions;
            }
        }
    }
}