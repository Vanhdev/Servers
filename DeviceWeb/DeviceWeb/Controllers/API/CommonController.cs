using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DeviceWeb
{
    [RoutePrefix("api/common")]
    public class CommonController : BaseApiController
    {
        public object Post(object info)
        {
            return info == null ? null : Execute(info);
        }
    }
}
