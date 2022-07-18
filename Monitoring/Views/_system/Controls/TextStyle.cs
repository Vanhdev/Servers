using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace System.Windows.Controls
{
    public class TextStyle : Style
    {
        public void Add(DependencyProperty prop, object value)
        {
            Setters.Add(new Setter(prop, value));
        }
    }
}
