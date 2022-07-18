using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Vst.GUI;

namespace System.Windows.Controls
{
    public partial class MyDropDownMenu : MyUserControl, IMenuContainer
    {
        MyNavMenuPanel _navContent;
        TextBlock _caption;
        DrawingPen _arrow;

        public bool IsHotKeyVisible 
        {
            get => _navContent.IsHotKeyVisible; 
            set => _navContent.IsHotKeyVisible = value;
        }
        public MyDropDownMenu()
        {
            var popup = new Primitives.Popup
            {
                Child = _navContent = new MyNavMenuPanel {
                },
            };
            var status = new Status();

            status.Changed += () => {
                popup.IsOpen = (status == 255);
            };

            popup.PlacementTarget = this;
            this.MouseLeftButtonUp += (s, e) => popup.IsOpen ^= true;
            this.MouseMove += (s, e) => {
                status[0] = true;
                status[1] = false;
            };

            this.MouseLeave += (s, e) => {
                var p = e.GetPosition(_navContent);
                Console.WriteLine(p);

                if (p.Y < 0)
                {
                    status[0] = false;
                }
            };
            popup.MouseMove += (s, e) => {
                status[1] = true;
                status[0] = false;
            };
            popup.MouseLeave += (s, e) => {
                var p = e.GetPosition(this);
                if (p.Y > this.ActualHeight || p.X > ActualWidth)
                {
                    status[1] = false;
                }
            };

            _navContent.MouseLeftButtonUp += (s, e) => status.Clear();

            Content.Add(_caption = new TextBlock {
                Margin = new Thickness(0, 0, 10, 0),
                VerticalAlignment = VerticalAlignment.Center,
            });


            _arrow = this.GetImage("Down");
            Content.Add(_arrow, 0, 1);

            Content.ColumnDefinitions[0].Width = GridLength.Auto;
        }

        protected override Size MeasureOverride(Size constraint)
        {
            _arrow.Stroke = this.Foreground;
            _arrow.Width = _arrow.Height = FontSize * 3 / 4;
            return base.MeasureOverride(constraint);
        }
        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            //_arrow.Pen = new Pen(this.Foreground, 1);
            //_arrow.InvalidateVisual();

            _navContent.MinWidth = ActualWidth;
        }

        public UIElementCollection Items => _navContent.Content.Children;
        public string Header
        {
            get => _caption.Text;
            set => _caption.Text = value;
        }
        public NavMenuItem Add(NavMenuItem item)
        {
            _navContent.Add(item);
            return item;
        }
        public NavMenuItem Add(MenuInfo info)
        {
            return Add(new NavMenuItem(info));
        }
    }
}
