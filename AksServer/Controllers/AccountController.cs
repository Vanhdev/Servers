using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vst;

namespace AksServer.Controllers
{
    class AccountController : BaseController
    {
        public object Login()
        {
            return SendContext(new Aks.Account().Login(Context.ValueContext) as DataContext);
        }
        public override object TryExecute(Context context)
        {
            var action = GetMethod(RequestContext.ActionName);
            if (action != null)
            {
                return action.Invoke(this, new object[] { });
            }

            var user = Aks.Account.FindActorByToken(context.Token);
            if (user != null)
            {
                return SendContext(user.Execute(RequestContext.ActionName, context) as DataContext);
            }
            return null;
        }

    }
}
