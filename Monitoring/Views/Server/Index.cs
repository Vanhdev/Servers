using System;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vst.GUI;

namespace Monitoring.Views.Server
{
    class Index : PageDataView
    {
        protected override MenuInfoList GetActions()
        {
            var lst = base.GetActions();
            lst.Add(new MenuInfo("Start", "Ctrl+R", null, StartServers));
            lst.Add(new MenuInfo("Stop", null, null, StopServers));

            return lst;
        }

        void StartServers()
        {
            CallSelectedItems(context => Request("Server/Start", context));
        }
        void StopServers()
        {
            CallSelectedItems(context => {
                var s = "";
                foreach (Models.ProcessInfo p in context.Value.Values)
                {
                    s += "    - " + p.Name + '\n';
                }
                if (DisplayConfirm("Stop the server(s)\n" + s) == true)
                {
                    Request("Server/Stop", context);
                }
            });
        }

        protected override void RenderCore()
        {
            base.RenderCore();
            foreach (var p in (IEnumerable<Models.ProcessInfo>)Model)
            {
                p.AliveChanged += (s, e) => {
                    RunRefresh();
                };
            }
        }

        protected override void RaiseItemSelected(object item)
        {
            Request("Server/Details", ((Models.ProcessInfo)item).Name);
        }
    }
}
