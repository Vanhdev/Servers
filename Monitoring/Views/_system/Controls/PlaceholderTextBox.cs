using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace VST.WPF
{
    public class PlaceholderTextBox : Grid
    {
        TextBlock _comment = new TextBlock {
            VerticalAlignment = VerticalAlignment.Bottom,
            Foreground = RGB.Parse("#88c"),
            Margin = new Thickness(0, 0, 0, 5),
        };
        TextBox _text = new TextBox {
            VerticalAlignment = VerticalAlignment.Bottom,
        };
        public PlaceholderTextBox()
        {
            Children.Add(_comment);
            Children.Add(_text);

            _text.TextChanged += (s, e) => {
                _comment.Visibility = string.IsNullOrEmpty(_text.Text) ? 
                    Visibility.Visible : Visibility.Hidden;

                this.TextChanged?.Invoke(_text.Text);
            };
        }
        public event Action<string> TextChanged;
        public TextBox Input => _text;
        public string Text
        {
            get => _text.Text;
            set => _text.Text = value;
        }
        public string Placeholder
        {
            get => _comment.Text;
            set => _comment.Text = value;
        }
    }
}
