using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ClientDemo
{
    class Led : Border
    {
        public Brush Color { get; set; } = Brushes.Green;
        public static Brush BaseColor { get; set; } = Brushes.LightGray;
        int _val;
        public int Value
        {
            get => _val;
            set
            {
                if (_val != value)
                {
                    _val = value;

                    this.Dispatcher.InvokeAsync(() => {
                        Background = _val != 0 ? Color : BaseColor;
                    });
                }
            }
        }
        public Led()
        {
            this.Background = BaseColor;
            this.Margin = new Thickness(3);
            Width = Height = 20;
            CornerRadius = new CornerRadius(Width / 2);
            HorizontalAlignment = HorizontalAlignment.Center;
        }
    }
}
