using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeviceWeb
{
    public class User : Aks.User
    {
        public string DeviceId
        {
            get => GetString("#deviceId");
            set => SetString("#deviceId", value);
        }
    }
}