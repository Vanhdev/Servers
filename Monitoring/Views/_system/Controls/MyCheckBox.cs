using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace System.Windows.Controls
{
    public class MyCheckBox : MyUserControl
    {
        class Mark : MyImage
        {
            protected override void OnPaint(double x0, double y0, double w, double h)
            {
                Start(0, 0).Offset(0, h + 1).Offset(w, 0).Offset(0, -h - 1);
                var br = Fill.Clone();
                br.Opacity = 0.25;

                Polygon(br, null, 0);

                var d = h / 6;
                var bottom = h - d;
                Start(w, 0)
                    .MoveTo(x0, bottom)
                    .MoveTo(d, y0 + 0.5)
                    .Offset(Thickness, -0.5)
                    .MoveTo(x0, bottom);
            }
        }
        Mark _marker;
        public MyCheckBox()
        {
            Content.Add(_marker = new Mark { Thickness = 0.85 });

            this.ApplyMouseClickAction(() => {
                SwicthValue();
            });
        }

        void SwicthValue()
        {
            IsChecked = _isChecked == true ? false : true;
            IsCheckedChanged?.Invoke(this, EventArgs.Empty);
        }

        bool? _isChecked;
        public event EventHandler IsCheckedChanged;
        public bool? IsChecked
        {
            get => _isChecked;
            set
            {
                if (value != _isChecked)
                {
                    _isChecked = value;
                    InvalidateMeasure();
                }
            }
        }

        protected override Size MeasureOverride(Size constraint)
        {
            if (_isChecked == true)
            {
                var w = this.Width - 1;
                _marker.Width = w;
                _marker.Height = w - 1;
                _marker.Stroke = Foreground;
                _marker.Fill = BorderBrush;

                _marker.Visibility = Visibility.Visible;
            }
            else
            {
                _marker.Visibility = Visibility.Collapsed;
            }
            return base.MeasureOverride(constraint);
        }
    }
}
