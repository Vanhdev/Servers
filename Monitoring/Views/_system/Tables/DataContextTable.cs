using Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Windows.Controls
{
    public class DataContextTable : DataTable
    {
        public override object[] GetDisplayItemValue(object item)
        {
            var lst = new List<object>();
            var context = System.DataContext.FromObject(item);
            foreach (DataTableColumn col in DataColumns)
            {
                if (col.IsVisible == false) continue;

                object v = context.GetString(col.Name);
                if (v != null)
                {
                    try
                    {
                        v = Convert.ChangeType(v, col.ColumnInfo.GetDataType());
                    }
                    catch
                    {

                    }
                }
                lst.Add(v);
            }
            return lst.ToArray();
        }
    }
}
