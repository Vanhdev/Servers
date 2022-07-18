using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Windows
{
    public interface IAppView : IDisposable, System.Mvc.IView
    {
        bool HasContent { get; }
        string MainCaption { get; }
    }
}
