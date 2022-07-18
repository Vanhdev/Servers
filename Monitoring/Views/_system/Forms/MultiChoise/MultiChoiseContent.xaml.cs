using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace System.Windows.Controls
{
    /// <summary>
    /// Interaction logic for MultiChoiseContent.xaml
    /// </summary>
    public partial class MultiChoiseContent : UserControl
    {
        public MultiChoiseContent()
        {
            InitializeComponent();
        }
    }

    class MultiChoiseForm : MyDialog
    {
        MultiChoiseContent main_content = new MultiChoiseContent();
        public DataTable Table { get; private set; } = new DataTable { IsHeaderVisible = false, };
        public MultiChoiseForm(string caption, string placeholder)
        {
            Body.Child = main_content;
            Caption.Text = caption;
            main_content.Search.Placeholder = placeholder;
            main_content.TableContent.Child = Table;
            SetActions("OK", "Close");
        }

        public void Begin(string filterPath, ValueFilter filter, object data, Action<IEnumerable<object>> callback)
        {
            //Table.ItemOpened += (s, e) => {
            //    callback?.Invoke(new object[] { s });
            //    base.RaiseAccept();
            //};
            Table.ItemsSource = (System.Collections.IEnumerable)data;

            var col = Table.Columns[filterPath];
            //col.Filter = filter;

            main_content.Search.TextChanged += (s) =>
            {
                if (s != string.Empty)
                {
                    //col.Filter.SetValue(s);
                    Table.ExecuteFilter();
                }
                else
                {
                    Table.ClearFilter();
                }
            };
            Completed += (items) => callback?.Invoke(items);

            ShowDialog();
        }

        public event Action<IEnumerable<object>> Completed;
        protected override void RaiseAccept()
        {
            var lst = Table.SelectedItems;
            if (lst.Length > 0) { Completed?.Invoke(lst); }
            base.RaiseAccept();
        }
    }

}
