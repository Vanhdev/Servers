using Mqtt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Vst.Server
{
    public abstract class SlaveServer : ServerBase
    {
        ShareMemory _masterMemory;
        public ShareMemory MasterMemory
        {
            get
            {
                if (_masterMemory == null)
                {
                    ShareMemory.Open(MasterName, m => _masterMemory = m);
                }
                return _masterMemory;
            }
        }
        public BsonData.Database CreateDefaultDB()
        {
            return CreateDatabase(ShareMemory.Name, null, 1000);
        }
        protected virtual string MasterName { get; set; } = "broker";
        protected virtual object ProcessInternalMessage(System.Mvc.RequestContext request, Context context)
        {
            var controller = _controllerMap.CreateController(request.ControllerName) as SlaveController;

            object response = null;
            if (controller != null)
            {
                try
                {
                    controller.RequestContext = request;
                    controller.Context = context;

                    response = controller.TryExecute(context);
                }
                catch (Exception e)
                {
                    Screen.Error("SlaveServer.cs MainThread");
                    Screen.Warning(e.ToString());
                }
            }
            return response;
        }
        protected override void MainThread(int interval)
        {
            this.ShareMemory.ReadObject<object>();
            this.ShareMemory.AsyncReading<Vst.Context>(interval, lst =>
            {
                foreach (var context in lst)
                {
                    var ts = new ThreadStart(() =>
                    {

                        Console.WriteLine(string.Format("[{0:HH:mm:ss}] ", DateTime.Now));
                        Screen.Info(context.Url);

                        // LOG
                        if (context.Value != null)
                        {
                            Screen.Success(context.Value.ToString());
                        }

                        // EXEC
                        var request = new System.Mvc.RequestContext(context.Url);
                        var response = this.ProcessInternalMessage(request, context);

                        // RESPONSE
                        if (response != null)
                        {
                            var internalName = context.Pop<string>("#internal");
                            if (internalName != null)
                            {
                                ShareMemory.Open(internalName, (sm) => sm.WriteObject(response));
                                return;
                            }

                            internalName = context.Pop<string>("#file");
                            if (internalName != null)
                            {
                                using (var sw = new System.IO.StreamWriter(internalName))
                                {
                                    try
                                    {
                                        sw.Write(((Context)response).Value);
                                    }
                                    catch
                                    {
                                        Screen.Error("Can not write to file: " + internalName);
                                    }
                                }
                                return;
                            }

                            MasterMemory.WriteObject(response);
                        }
                    });
                    new Thread(ts).Start();
                }
            });
        }
    }
    public abstract class SlaveController : System.Mvc.Controller
    {
        public Context Context { get; set; }
        protected string DefaultTopic => "response/default/";
        public virtual object TryExecute(Context context)
        {
            var action = GetMethod(RequestContext.ActionName);
            object res = null;
            if (action != null)
            {
                Context = context;
                res = action.Invoke(this, new object[] { });
            }
            return res;
        }

        public virtual PublishContext SendContext(string topic, string url, DataContext context)
        {
            if (url == null)
            {
                url = RequestContext.Combine('_');
            }
            var res = new Vst.Context
            {
                Action = url,
            };
            if (context != null)
            {
                res.Copy(context);
            }
            return new PublishContext
            {
                Topic = topic ?? (this.DefaultTopic + Context.ClientId),
                Value = res,
            };
        }
        public PublishContext SendContext(DataContext context)
        {
            return SendContext(context?.Pop<string>("#topic"), context?.Pop<string>("#url"), context);
        }
        public PublishContext SendContext(string topic, string url, Func<DataContext, object> exec)
        {
            return SendContext(topic, url, (DataContext)exec.Invoke(Context.ValueContext));
        }
        public PublishContext SendContext(Func<DataContext, object> exec)
        {
            return SendContext((DataContext)exec.Invoke(Context.ValueContext));
        }
        public PublishContext Response(object value)
        {
            return Response(null, value, 0, null, null);
        }
        public PublishContext Response(int code, string message, object value)
        {
            return Response(null, value, code, message, null);
        }
        public PublishContext Response(string topic, object value)
        {
            return Response(topic, value, 0, null, null);
        }
        public virtual PublishContext Response(string topic, object value, int code, string message, string url)
        {
            return SendContext(topic, url, new Vst.Context
            {
                Code = code,
                Value = value,
                Message = message,
            });
        }
        public PublishContext Error(int code, string message)
        {
            return Response(null, null, code, message, null);
        }
        public PublishContext Error(int code)
        {
            return Response(null, null, code, null, null);
        }
        public PublishContext Ok(string message)
        {
            return Response(null, null, 0, message, null);
        }
        public PublishContext Ok() { return Ok(null); }
    }
}
