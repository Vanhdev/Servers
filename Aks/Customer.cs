using System;
using System.Collections.Generic;
using System.Linq;

namespace Aks
{
    public partial class Admin
    {
        public object SetDevice(string topic, string deviceId)
        {
            DB.Devices.InsertOrUpdate(new BsonData.Document {
                ObjectId = deviceId,
            });
            return Ok();
        }
        public object RemoveDevice(string topic, string deviceId)
        {
            DB.Devices.Delete(new BsonData.Document {
                ObjectId = deviceId,
            });
            return Ok();
        }
    }
}