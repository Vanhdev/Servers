using Aks.Devices;
using System;
using System.Collections.Generic;
//using System.ComponentModel.Composition;
//using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vst;

namespace SecurityModel
{
    //[Export]
    public class V30 : StatusAnalyzer
    {
        public override Context ProcessDeviceMessage(string actionName, Device device, Context context)
        {
            try
            {
                string v = context.GetValue<string>();
                if (v == device.LastStatus)
                {
                    return null;
                }

                device.LastStatus = v;
                var status = device.Config.Status.Parse(Bitwise.Hex2Bytes(v));

                Screen.Warning(status.ToString());

                History.Add(device, status);

                return new Context { Value = "OK()" };
            }
            catch (Exception e)
            {
                Screen.Error("V30: " + e.Message);
            }
            return null;
        }
        public override Context ProcessUserMessage(string actionName, Device device, Context context)
        {
            var code = context.GetValue<string>();
            var action = device.Config.Control.FindCommand(code);
            if (action == null)
            {
                return null;
            }
            return new Context { Value = action.GetCommandLine() };
        }
        public override Context ProcessRemoteSetting(string actionName, Device device, Context context)
        {
            var action = context.ValueContext.ChangeType<RemoteAction>();
            var args = action.Arguments;
            var setting = device.Setting;

            switch (actionName.ToUpper())
            {
                case "PHONE":
                    device.UpdateSetting(action.FunctionName, action.Arguments);
                    break;

                case "PLAN":
                    var plan = setting.GetObject<DataContext>("PLAN");
                    if (plan == null)
                    {
                        plan = new DataContext();
                    }
                    plan.Push(args[0].ToString(), args[1].ToString());
                    device.UpdateSetting("PLAN", plan);
                    break;
            }
            Aks.DB.Devices.Update(device);

            return new Context { Value = action.GetCommandLine() };
        }
    }
}
