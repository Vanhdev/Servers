using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace System.Windows.Controls
{
    class VietFilterBox : FilterTextBox
    {
        public VietFilterBox(DataTable table, string name, string caption, double? width)
            : base(table, name, caption) 
        {
            if (width != null) this.Width = width.Value;
            //table.Columns[name].Filter = new UnicodeFilter();
        }
        public VietFilterBox(DataTable table, string name, string caption)
            : this(table, name, caption, 200)
        {

        }
    }
}
