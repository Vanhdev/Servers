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
    /// Interaction logic for SplitPanelLayout.xaml
    /// </summary>
    public partial class SplitPanelLayout : UserControl
    {
        public SplitPanelLayout()
        {
            InitializeComponent();
        }
        public UIElement Left
        {
            get => LeftPanel.Child;
            set => LeftPanel.Child = value;
        }
        public UIElement Right
        {
            get => RightPanel.Child;
            set => RightPanel.Child = value;
        }
        public UIElement ToolBar
        {
            get => ToolBarContent.Child;
            set => ToolBarContent.Child = value;
        }
        public double ToolBarHeight
        {
            get => ((Grid)Content).RowDefinitions[0].Height.Value;
            set => ((Grid)Content).RowDefinitions[0].Height = new GridLength(value);
        }
        public double LeftPanelWidth
        {
            get => ((Grid)RightPanel.Parent).ColumnDefinitions[0].Width.Value;
            set => ((Grid)RightPanel.Parent).ColumnDefinitions[0].Width = new GridLength(value);
        }
        public double RightPanelWidth
        {
            get => ((Grid)RightPanel.Parent).ColumnDefinitions[2].Width.Value;
            set => ((Grid)RightPanel.Parent).ColumnDefinitions[2].Width = new GridLength(value);
        }
    }
}
