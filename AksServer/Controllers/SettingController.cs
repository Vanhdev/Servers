using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aks;
using Aks.Devices;
using Vst;

namespace AksServer.Controllers
{
    partial class SettingController
    {
        protected override Context TryExecuteCore(Device device, Context context)
        {
            var token = context.Token;
            Screen.Warning(token);

            var user = Account.FindActorByToken(token);
            if (user == null)
            {
                return new Context { Code = 100, Message = "Token timeout" };
            }

            context = Analyzer.ProcessRemoteSetting(RequestContext.ActionName, device, context);
            Vst.Server.Memory.Broker.WriteObject(new Mqtt.PublishContext
            {
                Topic = device.GetDefaultTopic(),
                Value = context,
            });
            return user.Ok();
        }
    }
}
