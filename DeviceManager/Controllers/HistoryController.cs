using Aks.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DB = Aks.DB;

namespace DeviceManager
{
    class HistoryController : BaseController
    {
        public object All()
        {
            return DB.History.Find(DeviceId, null); 
        }
    }
}
