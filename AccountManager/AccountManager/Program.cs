using Aks;
using Mqtt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Mvc;
using System.Text;
using System.Threading.Tasks;
using Vst;

namespace AccountManager
{
    class Program : Vst.Server.SlaveServer
    {
        protected override object ProcessInternalMessage(RequestContext request, Context context)
        {
            var token = context.Token;
            object res = null;
            if (token != null)
            {
                var user = Account.FindActorByToken(token);
                res = user?.Execute(request.ActionName, context);
            }
            else
            {
                res = base.ProcessInternalMessage(request, context);
            }
            if (res == null || res is PublishContext) { return res; }

            var dx = (DataContext)res;
            var engine = new BaseController { 
                RequestContext = request,
                Context = context,
            };
            return engine.SendContext(dx.Pop<string>("#topic"), dx.Pop<string>("#url"), dx);
        }
        protected override void RaiseStarting(string path, string name)
        {
            base.RaiseStarting(path, "Account");
            DB.Main = CreateDefaultDB();

            Account.LoadActorConfig(path + "/config");

            DB.Accounts.Wait(() => {
                if (DB.Accounts.Count == 0)
                {
                    var acc = new Admin
                    {
                        UserName = DB.Admin,
                        Password = "Aks@1234",
                        Role = DB.Admin,
                    };
                    Account.CreateAccount(acc);
                }
            });
        }

        static void Main(string[] args)
        {
            var engine = new Program();
            engine.Start();
        }
    }
}
