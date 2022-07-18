using System;
using System.Collections.Generic;
using System.Linq;
using Vst.GUI;
using System.Text;
using System.Threading.Tasks;

namespace System.Windows.Controls
{
    static class DataTableExtension
    {
        public static void LoadTemplate(this DataTable table, params string[] columns) 
        {
            foreach (var name in columns)
            {
                table.AddTemplateColumn(name);
            }
        }       
        public static void LoadTemplate(this DataTable table, ColumnCollection fields)
        {
            foreach (var p in fields)
            {
                table.AddTemplateColumn(p.Key, p.Value);
            }
        }
        public static DataTableColumn AddTemplateColumn(this DataTable table, string name)
        {
            var field = GUI.Fields[name];
            if (field != null)
            {
                return table.AddTemplateColumn(name, field);
            }

            return table.Columns.Add(name);
        }
        public static DataTableColumn AddTemplateColumn(this DataTable table, string name, ColumnInfo field)
        {
            return table.Columns.Add(new DataTableTextColumn {
                ColumnInfo = field
            });
        }

    }
    public class TemplateDataTable : DataContextTable
    {
        public TemplateDataTable()
        {

        }
        public void Refresh()
        {
            base.InvalidateMeasure();
            base.InvalidateVisual();
        }

        public FilterTextBox CreateFilterText(string name, string caption, double? width)
        {
            var col = Columns[name];
            if (col == null) return null;

            //col.Filter = new UnicodeFilter();
            return new VietFilterBox(this, name, caption) { 
                Width = width ?? 150,
            };
        }
        public void ShowActions(Border border)
        {
            //border.Child = this.MenuBar;
        }
        public void SetChecked(IEnumerable<DataContext> source)
        {
            var map = new Dictionary<string, object>();
            foreach (var model in source)
            {
                map.Add(model.ObjectId, source);
            }
            foreach (var row in _rows)
            {
                var dst = (DataContext)row.DataContext;
                if (map.ContainsKey(dst.ObjectId))
                {
                    map.Remove(dst.ObjectId);
                    row.IsChecked = true;
                }
                else
                {
                    row.IsChecked = false;
                }
            }
        }
    }

    class TemplateDataTableLayout : Grid
    {
        public UIElement Top
        {
            get => ((Border)Children[0]).Child;
            set => ((Border)Children[0]).Child = value;
        }
        public TemplateDataTable Table => (TemplateDataTable)Children[1];
        public TemplateDataTableLayout()
        {
            Border border = new Border();
            this.Add(border);
            this.Add(new TemplateDataTable(), 1, 0);

            Table.ShowActions(border);
            this.RowDefinitions[0].Height = GridLength.Auto;
        }

        protected override Size MeasureOverride(Size constraint)
        {
            if (Fields != null && Table.Columns.Count < 3)
            {
                foreach (var name in Fields.Split(',', ';'))
                {
                    Table.AddTemplateColumn(name);
                }
                if (SearchBox != null)
                {
                    var v = SearchBox.Split(',', ';');
                    Table.CreateFilterText(v[0], v[1], SearchBoxWidth);
                }
            }
            return base.MeasureOverride(constraint);
        }
        public string Fields { get; set; }
        public string SearchBox { get; set; }
        public double? SearchBoxWidth { get; set; }
    }
}
