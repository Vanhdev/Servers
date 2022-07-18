using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AksServer
{
    using Aks;
    using Controllers;

    class Program : Vst.Server.SlaveServer
    {
        protected override void RaiseStarting(string path, string name)
        {
            base.RaiseStarting(path, "AKS");

            DB.Main = this.CreateDefaultDB();

            var configPath = path + "/config";

            var models = DB.DeviceVersions.LoadConfigFolder(configPath + "/models");
            Aks.Devices.StatusAnalyzer.CreateAnalyzers(path, models);

            Account.LoadActorConfig(configPath + "/actors");
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


            Screen.Info(string.Format($"{DB.Devices.Count} device(s) in system."));
        }
        static void Main(string[] args)
        {
            var engine = new Program();
            engine.Start();
        }
    }
}
