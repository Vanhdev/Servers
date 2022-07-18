using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManager.Controllers
{
    class AccountController : BaseController
    {
        public object Login()
        {
            return new Aks.Account().Login(Context.ValueContext);
        }
    }
}
