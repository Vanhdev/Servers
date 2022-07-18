using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vst;

namespace AksServer.Controllers
{
    class TokenBase : RemoteController
    {
        public object TryExecute(string token, Context context)
        {
            var user = Aks.Account.FindActorByToken(token);
            if (user == null) { return Error(500); }

            var res = user.Execute(RequestContext.ActionName, context);
            return SendContext((DataContext)res);
        }
        public override object TryExecute(Context context)
        {
            var token = context.Token;
            if (token == null) { return Error(500); }

            return TryExecute(token, context);
        }
    }
}
