using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Aks.Devices
{
    //public class Version : DataContext
    //{
    //    public string GetPublishTopic(Device device)
    //    {
    //        return string.Format(GetString("#publish"), device.ObjectId);
    //    }
    //    public string GetSubscribeTopic(Device device)
    //    {
    //        return string.Format(GetString("#subscribe"), device.ObjectId);
    //    }
    //    public void Execute(Device device, DataContext context)
    //    {
    //        string url = context.Pop<string>("url");
    //        context.Pop<string>("cid");

    //        if (url != null)
    //        {
    //            var s = url.Split('/');
    //            var action = s[s.Length - 1].ToLower();

    //            context.Push("action", action);
    //            ExecuteCore(device, context, action);
    //        }
    //    }
    //    public Action<Device, DataContext, string> ExecuteCore;
    //    public Func<DataContext, string> GetHistoryDetails;
    //    protected static int Hex2Dec(char c)
    //    {
    //        if (c >= 'a') { return c - 'a' + 10; }
    //        if (c >= 'A') { return c - 'A' + 10; }

    //        return c & 15;
    //    }
    //    protected static Bitwise GetStatus(char c)
    //    {
    //        return Hex2Dec(c);
    //    }
    //    static public Version GetVersion(string name)
    //    {
    //        //var key = name.ToLower();
    //        //if (_versions.ContainsKey(key))
    //        //{
    //        //    var v = _versions[key] as Version;
    //        //    v.ExecuteCore = V30.ExecuteContext;

    //        //    return (Version)v.MemberwiseClone();
    //        //}
    //        return null;
    //    }
    //    static public void LoadVersions(string filename)
    //    {
    //        using (var sr = new StreamReader(filename))
    //        {
    //            var data = Parse(sr.ReadToEnd());
    //            _versions = new DataContext();

    //            foreach (var p in data)
    //            {
    //                var v = FromObject<Version>(p.Value);
    //                v.ObjectId = p.Key;

    //                _versions.Add(p.Key.ToLower(), v);
    //            }
    //        }
    //    }
    //    static DataContext _versions;
    //}
}
