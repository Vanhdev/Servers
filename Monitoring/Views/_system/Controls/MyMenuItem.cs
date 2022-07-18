using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vst.GUI;

namespace System.Windows.Controls
{
    public interface IMenuContainer
    {
        UIElementCollection Items { get; }
    }
    public partial class MyMenuBar : MyUserControl<StackPanel>, IMenuContainer
    {
        public MyMenuBar()
        {
            Content.Orientation = Orientation.Horizontal;
        }
        public UIElementCollection Items => Content.Children;

    }

    public partial class MyNavMenuPanel : MyUserControl<StackPanel>
    {
        public bool IsHotKeyVisible { get; set; } = true;
        protected override Size MeasureOverride(Size constraint)
        {
            if (IsHotKeyVisible)
            {
                double w = 0;
                double x = 0;
                foreach (NavMenuItem item in Content.Children)
                {
                    if (item.HotKey != null && item.Content.ColumnDefinitions.Count < 2)
                    {
                        item.Measure(constraint);
                        w = Math.Max(w, item.DesiredSize.Width);
                        var hkText = new TextBlock
                        {
                            Text = item.HotKey,
                            Padding = new Thickness(20, 0, 10, 0),
                        };
                        item.Content.Add(hkText, 0, 1);
                        hkText.Measure(constraint);

                        x = Math.Max(x, hkText.DesiredSize.Width);
                    }
                }
                if (w > 0)
                {
                    var s = new Size(w + x, 0);
                    foreach (NavMenuItem item in Content.Children)
                    {
                        if (item.Content.ColumnDefinitions.Count > 0)
                        {
                            item.Content.ColumnDefinitions[0].Width = new GridLength(w - 40);
                        }
                        s.Height += item.DesiredSize.Height;
                    }
                    return s;
                }
            }
            return base.MeasureOverride(constraint);
        }
        public NavMenuItem Add(MenuInfo info)
        {
            return this.Add(new NavMenuItem(info));
        }
        public NavMenuItem Add(NavMenuItem item)
        {
            if (item.IsBeginGroup)
            {
                item.BorderBrush = this.BorderBrush;
                item.BorderThickness = new Thickness(0, 1, 0, 0);
            }
            Content.Children.Add(item);
            return item;
        }
    }
    public class MyMenuExpander : MyExpander<MyNavMenuPanel>
    {
        public MyMenuExpander()
        {
            IsExpanded = true;
        }
    }
    public partial class MyMenuItem : MyButtonBase
    {
        public MyMenuItem() { }
        public MyMenuItem(MenuInfo info)
        {
            Url = info.Url;
            Text = info.Text;
            HotKey = info.HotKey;
            if (info.Invoke != null)
            {
                Click += (s, e) => info.Invoke();
            }
        }
    }

    public partial class NavMenuItem : MyMenuItem
    {
        public bool IsBeginGroup { get; set; }
        public NavMenuItem()
        {
        }
        public NavMenuItem(MenuInfo info) : base(info)
        {
            IsBeginGroup = info.BeginGroup;
        }
    }
    //public interface INavMenuContent
    //{
    //    Panel GetNavMenuContent();
    //    string Header { get; set; }
    //}


}
