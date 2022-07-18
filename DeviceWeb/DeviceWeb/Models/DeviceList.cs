using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeviceWeb
{
    using Models;
    public class DeviceList : List<Device>
    {
        public Device this[string id] => base.Find(x => x.ObjectId == id);
    }
}