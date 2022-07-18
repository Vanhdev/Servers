using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Aks
{
    public class User : Account
    {
        public object RemoteControl(DataContext context)
        {
            var deviceId = context.Pop<string>("#deviceId");
            // TODO: check role here

            return context;
        }
        public object RemoteSetting(DataContext context)
        {
            return context;
        }
    }
}
