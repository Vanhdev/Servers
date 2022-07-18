using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vst.Server;

namespace Monitoring.Controllers
{
    class HomeController : BaseController
    {
        public object Index()
        {
            return Redirect("Server");
        }
    }
}
