using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace System.Windows.Controls
{

    public abstract class MyButtonBase : MyUserControl
    {
        TextBlock _caption;
        public TextBlock Caption
        {
            get 
            {
                if (_caption == null)
                {
                    Content.Children.Add(_caption = new TextBlock());
                }
                return _caption; 
            }
        }
        public string Text
        {
            get => _caption?.Text;
            set => Caption.Text = value;
        }
        public static readonly DependencyProperty UrlProperty =
            DependencyProperty.Register("Url", typeof(string), typeof(MyButtonBase));
        public string Url
        {
            get => (string)GetValue(UrlProperty);
            set => SetValue(UrlProperty, value);
        }

        public event EventHandler Click;
        public override void Activate()
        {
            Click?.Invoke(this, EventArgs.Empty);
            this.Request();
        }
        public MyButtonBase()
        {
            this.ApplyMouseClickAction(null);
        }
    }

    public class MyExpanderButton : MyUserControl
    {
        TextBlock _caption;
        DrawingPen _arrow;
        public DrawingPen Arrow => _arrow;
        public string Text
        {
            get => _caption.Text;
            set => _caption.Text = value;
        }
        bool _expanded;

#pragma warning disable CS0067
        public event EventHandler Click;
        public event EventHandler IsExpandedChanged;
#pragma warning restore CS0067

        public bool IsExpanded
        {
            get => _expanded;
            set
            {
                if (_expanded != value)
                {
                    _expanded = value;
                    InvalidateMeasure();

                    IsExpandedChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }
        public MyExpanderButton()
        {
            Content.Add(_caption = new TextBlock { 
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 10, 0)
            });
            Content.Add(_arrow = this.GetImage("Down"), 0, 2);

            Content.ColumnDefinitions[1].Width = new GridLength(20);
            Content.ColumnDefinitions[2].Width = GridLength.Auto;

            this.ApplyMouseClickAction(() => {
                IsExpanded ^= true;
            });
        }

        protected override Size MeasureOverride(Size constraint)
        {
            _arrow.Width = _arrow.Height = FontSize;
            _arrow.Stroke = Foreground;

            return base.MeasureOverride(constraint);
        }
    }
}
