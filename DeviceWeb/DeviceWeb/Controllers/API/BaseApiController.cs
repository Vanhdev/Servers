using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Web.Http;
using BsonData;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DeviceWeb
{

    //public class BaseApiController : ApiController
    //{
    //    //protected virtual object Execute(object requestContext)
    //    //{
    //    //    var context = JObject.FromObject(requestContext).ToObject<DataContext>();
    //    //    var url = context.Pop<string>("#url");
    //    //    if (url == null) return null;

    //    //    var s = url.Split('/');
    //    //    var token = context.GetString("#token");

    //    //    var actor = new Aks.User();
    //    //    if (!string.IsNullOrEmpty(token))
    //    //    {
    //    //        Aks.Account.FindActorByToken(token);
    //    //        if (actor == null)
    //    //        {
    //    //            return null;
    //    //        }
    //    //    }

    //    //    var actionName = s[1].ToLower();
    //    //    var method = actor.GetType().FindMethod(actionName, typeof(DataContext));

    //    //    try
    //    //    {
    //    //        if (method != null)
    //    //        {
    //    //            var res = method.Invoke(actor, new object[] { context }) as DataContext;
    //    //            res?.Push("#url", url.ToLower().Replace('/', '_'));

    //    //            return res;
    //    //        }
    //    //    }
    //    //    catch (Exception ex)
    //    //    {
    //    //        Console.WriteLine(ex.Message);
    //    //    }
    //    //    return null;
    //    //}

    //    static Vst.Context unknowError = new Vst.Context { Code = 400, Message = "Unknow Error" };

    //    //protected virtual Vst.Context Execute(object requestContext)
    //    //{
    //    //    Vst.Context res = null;
    //    //    try
    //    //    {
    //    //        var context = DataContext.FromObject(requestContext)
    //    //            .ChangeType<Vst.Context>();

    //    //        var file = InternalReponse.CreateFile();
    //    //        context.SetString("#file", file.FullName);

    //    //        if (!Vst.Server.ShareMemory.Open(context.Pop<string>("#server"),
    //    //            sm => sm.WriteObject(context)))
    //    //        {
    //    //            return new Vst.Context { Code = 503, Message = "Server not found" };
    //    //        }

    //    //        int count = 100;
    //    //        while (--count > 0)
    //    //        {
    //    //            Thread.Sleep(100);
    //    //            if (file.Exists)
    //    //            {
    //    //                try
    //    //                {
    //    //                    using (var sr = file.OpenText())
    //    //                    {
    //    //                        var content = sr.ReadToEnd();
    //    //                        res = DataContext.Parse<Vst.Context>(content);
    //    //                    }
    //    //                    file.Delete();

    //    //                    return res;
    //    //                }
    //    //                catch
    //    //                {
    //    //                    res = new Vst.Context { Code = 100, Message = "Server is busy" };
    //    //                }
    //    //            }
    //    //        }
    //    //    }
    //    //    catch (Exception e)
    //    //    {
    //    //        res = new Vst.Context { Code = 400, Message = e.Message };
    //    //    }
    //    //    return res ?? unknowError;
    //    //}

    //    protected virtual Vst.Context Execute(object requestContext)
    //    {
    //        Vst.Context res = null;
    //        try
    //        {
    //            var file = Aks.InternalContext.SendRequest(requestContext.ToString());
    //            int count = 30;
    //            while (--count > 0)
    //            {
    //                Thread.Sleep(100);
    //                if (file.Exists)
    //                {
    //                    try
    //                    {
    //                        using (var sr = file.OpenText())
    //                        {
    //                            var content = sr.ReadToEnd();
    //                            res = DataContext.Parse<Vst.Context>(content);
    //                        }
    //                        file.Delete();

    //                        return res;
    //                    }
    //                    catch
    //                    {
    //                        res = new Vst.Context { Code = 100, Message = "Server is busy" };
    //                    }
    //                }
    //            }
    //        }
    //        catch (Exception e)
    //        {
    //            res = new Vst.Context { Code = 400, Message = e.Message };
    //        }
    //        return res ?? unknowError;
    //    }

    //}
}

namespace DeviceWeb
{
    using uPLibrary.Networking.M2Mqtt;
    public class BaseApiController : ApiController
    {

        static Vst.Context unknowError = new Vst.Context { Code = 400, Message = "Unknow Error" };
        static DataContext _response;
        static bool _busy;

        static MqttClient _client;
        protected virtual DataContext Execute(object requestContext)
        {
            if (_client == null)
            {
                string cid = Guid.NewGuid().ToString();
                //_client = new MqttClient("system.aks.vn");
                _client = new MqttClient("localhost");
                _client.Connect(cid);

                _client.Subscribe(new string[] { "response/default/" + cid }, new byte[] { 0 });
                _client.MqttMsgPublishReceived += (s, e) => {
                    _response = DataContext.Parse(e.Message.UTF8());
                    _busy = false;
                };
            }

            while (_busy) { }

            var context = DataContext.FromObject(requestContext);
            var topic = (string)context.Pop("#server");

            _response = null;
            _busy = true;
            _client.Publish(topic, context.ToString().UTF8());

            Vst.Context res = null;
            try
            {
                int count = 30;
                while (--count > 0)
                {
                    Thread.Sleep(100);
                    if (_response != null)
                    {
                        _busy = false;
                        return _response;
                    }
                }
            }
            catch (Exception e)
            {
                res = new Vst.Context { Code = 400, Message = e.Message };
            }

            _busy = false;
            return res ?? unknowError;
        }

    }

}
