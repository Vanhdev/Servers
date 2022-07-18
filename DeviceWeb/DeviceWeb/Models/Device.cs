using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeviceWeb.Models
{
    public class Device : Aks.Devices.Device
    {
        new public Aks.Devices.DeviceConfig Config
        {
            get => base.Config;
            set
            {
                base.Config = value;
                Push("config", value);
            }
        }
        public string ToBase64()
        {
            return Convert.ToBase64String(this.ToString().UTF8());
        }
    }
}