using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monitoring.Views
{
    internal abstract class BaseError : System.Mvc.IView
    {
        public System.Mvc.ViewDataDictionary ViewBag { get; set; }
        public object Content { get => null; }
        abstract protected string GetMessageText(string code);
        public void Render(object model)
        {
            System.Windows.Controls.MyDialog.Alert(GetMessageText((string)model), "System");
        }
    }
}
