using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aks
{
    public partial class Admin : User
    {
        public object SetUser(Vst.Context context)
        {
            return CreateApiResponse(CreateAccount(context), null, null);
        }
    }
}
