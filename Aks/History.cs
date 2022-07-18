using BsonData;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Aks.Devices
{
    public class DeviceEvent
    {
        public DateTime Time { get; set; }
        public DataContext Content { get; set; }
    }
    public class History : Document
    {
        static string GetHistoryKey(string deviceId) => deviceId + "_his";
        static public void Add(Device device, DataContext context)
        {
            device.LastHistory = context;

            DB.History.Wait(() => {
                var key = GetHistoryKey(device.ObjectId);
                var item = DB.History.Find<History>(key);
                
                if (item == null)
                {
                    item = new History();
                    DB.History.Insert(key, item);
                }

                var time = DateTime.Now;
                var eventKey = time.ToString("yyyyMM");

                var ev = new DeviceEvent
                {
                    Time = time,
                    Content = context,
                };
                var lst = item.GetArray<List<DeviceEvent>>(eventKey);
                if (lst == null)
                {
                    lst = new List<DeviceEvent>();
                }
                lst.Insert(0, ev);
                item.Push(eventKey, lst);

                DB.History.Update(item);
            });
        }
        static public History Find(Device device)
        {
            return Find(device.ObjectId);
        }
        static public History Find(string deviceId)
        {
            History res = new History();
            DB.History.Wait(() => res = DB.History.Find<History>(GetHistoryKey(deviceId)));

            return res;
        }
        //static string GetHistoryDetails(DataContext context)
        //{
        //    return string.Join(", ", context.Keys);
        //}
        //static public DataContext GetHistory(Device device)
        //{
        //    History lst = new History();
        //    DB.History.Find(device.ObjectId, item =>
        //    {
        //        var context = new DataContext();
        //        context.Copy(item);
        //        context.Remove(typeof(ObjectId).Name);

        //        foreach (var p in context)
        //        {
        //            foreach (var record in JArray.Parse((string)p.Value).ToObject<List<DataContext>>())
        //            {
        //                var time = record.Pop<DateTime>("time");
        //                var func = device.Version.GetHistoryDetails ?? GetHistoryDetails;

        //                lst.Push(time.ToString("yyyy-MM-ddTHH:mm:ss"), func(record));
        //            }
        //        }
        //    });
        //    return lst;
        //}
    }
}
namespace Aks
{
    partial class DB
    {
        static Collection _history;
        public static Collection History
        {
            get
            {
                if (_history == null)
                {
                    _history = GetCollection<Aks.Devices.History>();
                }
                return _history;
            }
        }
    }
}