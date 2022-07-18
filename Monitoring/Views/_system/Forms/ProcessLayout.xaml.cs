using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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
    /// Interaction logic for ProcessLayout.xaml
    /// </summary>
    public partial class ProcessLayout : UserControl
    {
        public ProcessLayout()
        {
            InitializeComponent();
        }
    }
    internal class ProgressDialog : MyDialog
    {
        public ProgressDialog(string caption, int total, int delay)
        {
            Caption.Text = caption;
            var pb = new ProcessLayout
            {
            };
            pb.Progress.Maximum = total;
            this.Body.Child = pb;

            var timer = new Threading.DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(delay),
            };
            timer.Tick += (s, e) => {
                if (pb.Progress.Value == total)
                {
                    timer.Stop();
                    Cancel.Content = "Close";

                    var report = Report?.Invoke();
                    if (report != null)
                    {
                        pb.Progress.Visibility = Visibility.Hidden;
                        pb.Value.Text = report;
                    }
                    else
                    {
                        this.Close();
                    }
                    return;
                }
                pb.Value.Text = OneStep((int)pb.Progress.Value);
                ++pb.Progress.Value;
            };

            Cancel.Click += (s, e) => timer.Stop();
            timer.Start();
        }

        public Func<int, string> OneStep;
        public Func<string> Report;
    }
}
