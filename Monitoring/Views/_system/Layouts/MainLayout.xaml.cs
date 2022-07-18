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
using Vst.GUI;

namespace System.Windows.Controls
{
    /// <summary>
    /// Interaction logic for MainLayout.xaml
    /// </summary>
    public partial class MainLayout : UserControl
    {
        public MainLayout()
        {
            InitializeComponent();

            //menuContent.Width = splitPanel.ColumnDefinitions[0].Width.Value;
            //splitPanel.ColumnDefinitions[0].Width = GridLength.Auto;
            double leftSize = 0;
            menuContent.SizeChanged += (s, e) => { 
                if (e.NewSize.Width != 0)
                {
                    leftSize = e.NewSize.Width;
                }
            };
            btnMenu.Click += (s, e) => {
                var w = splitPanel.ColumnDefinitions[0].Width.Value;
                splitPanel.ColumnDefinitions[0].Width = new GridLength(w == 0 ? leftSize : 0);
            };


            MyKeyboard.AddRange(this.banner.Children);
        }
        public void UpdateView(IAppView view)
        {
            var content = (UIElement)view.Content;
            string caption = view.MainCaption?.ToUpper();
            if (!string.IsNullOrEmpty(caption))
            {
                main_caption.Content = caption;
            }
            main_content.Child = content;
        }
        public void CreateMenu(MenuInfo mainMenu)
        {
            main_caption.Content = mainMenu.Text;
            foreach (var info in mainMenu.Childs)
            {
                var item = new MyMenuExpander
                {
                    Header = info.Text,
                };
                if (info.HasChilds)
                {
                    foreach (var child in info.Childs)
                    {
                        child.BeginGroup = true;
                        var nav = item.Body.Add(child);
                        nav.Background = System.Windows.Media.Brushes.Transparent;
                    }
                }
                menuContent.Children.Add(item);
            }
        }
    }
}
