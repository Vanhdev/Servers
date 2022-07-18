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
            name = "NICHIRIN";
            base.RaiseStarting(path, name);

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
                        Password = char.ToUpper(name[0]) + name.Substring(1).ToLower() + "@1234",
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
