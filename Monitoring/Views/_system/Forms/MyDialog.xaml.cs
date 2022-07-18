using System;
using System.Collections.Generic;
using System.Linq;
using Vst.GUI;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Models;

namespace System.Windows.Controls
{
    /// <summary>
    /// Interaction logic for MyDialog.xaml
    /// </summary>
    public partial class MyDialog : Window
    {
        public MyDialog()
        {
            InitializeComponent();

            OK.Visibility = Visibility.Collapsed;
            Cancel.Click += (s, e) => DialogResult = false;
            PreviewKeyDown += (s, e) =>
            {
                switch (e.Key)
                {
                    case Key.Escape: DialogResult = false; break;
                    case Key.Return: RaiseAccept(); break;
                }
            };
            //this.SizeToContent = SizeToContent.Height;
        }
        protected void SetActions(string ok, string cancel)
        {
            if (cancel != null)
            {
                Cancel.Content = cancel;
            }
            if (ok != null)
            {
                //Cancel.Margin = OK.Margin;
                //OK.Width = Cancel.Width;
                OK.Content = ok;
                OK.Visibility = Visibility.Visible;
                OK.Click += (s, e) => {
                    RaiseAccept();
                };
            }
        }
        protected bool? ShowDialogCore(UIElement content, string caption, string cancel, string ok)
        {
            Body.Child = content;
            Caption.Text = caption;

            SetActions(ok, cancel);
            return ShowDialog();
        }
        protected virtual void RaiseAccept()
        {
            DialogResult = true;
        }
        protected bool? ShowTextCore(string text, string caption, string cancel, string ok)
        {
            return ShowDialogCore(new TextBlock {
                Text = text,
                MinWidth = 350,
                MinHeight = 100,
            }, caption, cancel, ok);
        }

        bool _bodyCalculated = false;
        protected override Size MeasureOverride(Size availableSize)
        {
            if (_bodyCalculated == false)
            {
                _bodyCalculated = true;

                Body.Measure(new Size(availableSize.Width, 600));
                var sz = Body.DesiredSize;

                Left += (availableSize.Width - sz.Width) / 2;
                Top -= sz.Height / 2;

                this.Width = sz.Width;
                this.Height += sz.Height;

            }

            return base.MeasureOverride(availableSize);
        }

        public static void Alert(string text, string caption)
        {
            var dlg = new MyDialog();
            dlg.ShowTextCore(text, caption, "OK", null);
        }
        public static bool? Confirm(string text, string caption, string ok, string cancel)
        {
            var dlg = new MyDialog();
            return dlg.ShowTextCore(text, caption, cancel, ok);
        }
    }

    public class EditForm : MyDialog
    {
        protected FormContent _formContent = new FormContent();
        public EditForm()
        {
            SetActions("OK", null);
            Body.Child = _formContent;
        }
        public void LoadTemplate(string name)
        {
            var info = GUI.Forms[name];

            _formContent.LoadTemplate(info.Fields);
            _formContent.Width = info.Width;

            this.Caption.Text = info.Caption;
        }

        public event EventHandler Completed;
        Action completedCallback;
        
        protected override void RaiseAccept()
        {
            var data = _formContent.EditedValue;
            if (data == null)
            {
                return;
            }

            var context = (DataContext)this.DataContext;
            if (context != null)
            {
                foreach (var p in data)
                {
                    context.Push(p.Key, p.Value);
                }
            }
            else
            {
                context = data;
            }    

            completedCallback?.Invoke();
            Completed?.Invoke(context, EventArgs.Empty);

            base.RaiseAccept();
        }
        public void BeginEdit(DataContext context, Action completedCallback)
        {
            this.completedCallback = completedCallback;
            if (context == null)
            {
                context = new DataContext();
            }
            this.DataContext = context;
            foreach (var p in context)
            {
                var e = _formContent.Editors[p.Key];
                if (e != null)
                {
                    e.Value = p.Value;
                }
            }
        }
    }
}
