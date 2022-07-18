using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Windows.Controls
{
    public class MyExpander<T> : MyUserControl<StackPanel>
        where T : FrameworkElement, new()
    {
        MyExpanderButton _button = new MyExpanderButton();
        T _body = new T();

        public string Header
        {
            get => _button.Text;
            set => _button.Text = value;
        }
        public bool IsExpanded
        {
            get => _button.IsExpanded;
            set => _button.IsExpanded = value;
        }
        public MyExpanderButton Expender => _button;
        public T Body => _body;

        public static int AnimationSteps { get; set; } = 16;

        public MyExpander()
        {
            Content.Children.Add(_button);
            Content.Children.Add(_body);


            double bodyHeight = 0;
            double bodyCollapse = 0;
            double h = 0;
            double d = 0;

            _button.Arrow.Rotating += (s, e) => {
                h += d;
                if (d < 0)
                {
                    if (h < 0)
                    {
                        bodyCollapse = _body.Height;
                        _body.Visibility = Visibility.Collapsed;
                        return;
                    }
                }
                else
                {
                    if (h > bodyHeight)
                    {
                        _body.Height += bodyCollapse;
                        return;
                    }
                }

                _body.Height = h;
            };
            _button.IsExpandedChanged += (s, e) => {

                if (bodyHeight == 0)
                {
                    bodyHeight = _body.ActualHeight;
                    if (bodyHeight == 0)
                    {
                        return;
                    }
                }

                h = _body.DesiredSize.Height;
                d = bodyHeight / AnimationSteps;

                if (IsExpanded == false) d = -d;

                _body.Visibility = Visibility.Visible;
                _button.Arrow.BeginRotate(0.2, IsExpanded ? -180 : 180, AnimationSteps);
            };
        }
    }
}
