using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vst.GUI
{
    public class ColumnInfo
    {
        public string Name { get; set; }
        public string Caption { get; set; }
        public string DisplayFormat { get; set; }
        public string BackgroundColor { get; set; }
        public string Color { get; set; }
        public double? Width { get; set; }
        public double? FontSize { get; set; }
        public int? TextAlignment { get; set; }
        public bool? IsVisible { get; set; }
        public string DataType { get; set; }
        public string ClassName { get; set; }
        public void Copy(ColumnInfo src)
        {
            foreach (var p in this.GetType().GetProperties())
            {
                object v = p.GetValue(this);
                if (v == null || (p.PropertyType.IsValueType
                    && v.Equals(Activator.CreateInstance(p.PropertyType))))
                {
                    p.SetValue(this, p.GetValue(src));
                }
            }
        }
        Type _dataType;
        public Type GetDataType()
        {
            if (_dataType == null)
            {
                _dataType = DataType == null ? 
                    typeof(string) : Type.GetType("System." + DataType);
            }
            return _dataType;
        }
    }
}
