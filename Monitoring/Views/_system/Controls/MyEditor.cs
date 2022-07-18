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
    public static class KeyExtension
    {
        static bool toUnicodeIsTrue;
        public static bool ToUnicodeIsTrue => toUnicodeIsTrue;

        public static char ToChar(this Key key)
        {
            toUnicodeIsTrue = true;
            char ch = ' ';

            // First, you need to get the VirtualKey code. Thankfully, there’s a simple class 
            // called KeyInterop, which exposes a static method VirtualKeyFromKey 
            // that gets us this information
            int virtualKey = KeyInterop.VirtualKeyFromKey(key);
            //Then, we need to get the character. This is much trickier. 
            //First we have to get the keyboard state and then we have to map that VirtualKey 
            //we got in the first step to a ScanCode, and finally, convert all of that to Unicode, 
            //because .Net doesn’t really speak ASCII
            byte[] keyboardState = new byte[256];
            GetKeyboardState(keyboardState);

            uint scanCode = MapVirtualKey((uint)virtualKey, MapType.MAPVK_VK_TO_VSC);
            StringBuilder stringBuilder = new StringBuilder(2);

            int result = ToUnicode((uint)virtualKey, scanCode, keyboardState, stringBuilder, stringBuilder.Capacity, 0);
            switch (result)
            {
                case -1:
                    toUnicodeIsTrue = false;
                    break;
                case 0:
                    toUnicodeIsTrue = false;
                    break;
                case 1:
                    {
                        ch = stringBuilder[0];
                        break;
                    }
                default:
                    {
                        ch = stringBuilder[0];
                        break;
                    }
            }
            return ch;
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool GetKeyboardState(byte[] lpKeyState);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern uint MapVirtualKey(uint uCode, MapType uMapType);
        public enum MapType : uint
        {
            MAPVK_VK_TO_VSC = 0x0,
            MAPVK_VSC_TO_VK = 0x1,
            MAPVK_VK_TO_CHAR = 0x2,
            MAPVK_VSC_TO_VK_EX = 0x3,
        }
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int ToUnicode(
            uint wVirtKey,
            uint wScanCode,
            byte[] lpKeyState,
            [System.Runtime.InteropServices.Out, System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPWStr, SizeParamIndex = 4)]
        StringBuilder pwszBuff,
            int cchBuff,
            uint wFlags);
    }
}

namespace System.Windows.Controls
{
    public interface IEditor
    {
        object Value { get; set; }
        string Name { get; set; }
        string Text { get; set; }
        double Size { get; set; }
        FrameworkElement Editor { get; }
        void SetOptions(string value);
        bool Focus();
    }

    public abstract class MyEditor : MyUserControl, IEditor
    {
        public double Size { get; set; } = 100;
        public string Text 
        { 
            get => (string)_caption.Content; 
            set => _caption.Content = value; 
        }
        public object Value
        {
            get
            {
                var v = GetValueCore();
                if (v != null && v.Equals(string.Empty))
                {
                    v = null;
                }
                return v;
            }
            set => SetValueCore(value); 
        }
        public FrameworkElement Editor { get; private set; }
        protected Label _caption;

        protected abstract FrameworkElement CreateEditorCore();
        protected abstract object GetValueCore();
        protected abstract void SetValueCore(object value);
        public abstract void SetOptions(string value);
        public MyEditor()
        {
            this.Content.Add(Editor = CreateEditorCore());
            this.Content.Add(_caption = new Label(), 1, 0);
        }
    }
    public class MyTextBox : MyUserControl
    {
        public class MyInputPopupScroller : MyUserControl {

            new public object Content
            {
                get => ((ScrollViewer)base.Content.Children[0]).Content;
                set => ((ScrollViewer)base.Content.Children[0]).Content = value;
            }

            public MyInputPopupScroller()
            {
                var sv = new ScrollViewer { 
                    VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                    Focusable = false,
                };
                base.Content.Add(sv);

            }
        }
        public class MyInputPopupItem : MyButtonBase
        {

        }
        public class MyInputPopup : Primitives.Popup
        {
            StackPanel _itemsContent;
            public MyInputPopup()
            {
                this.Child = new MyInputPopupScroller
                {
                    Content = _itemsContent = new StackPanel { 
                        Focusable = true,
                    },
                };
                _itemsContent.MouseDown += (s, e) => {
                    var p = e.GetPosition(_itemsContent);
                    double y = 0;
                    foreach (MyInputPopupItem item in Children)
                    {
                        if (item.IsVisible == false) continue;

                        if (y + item.ActualHeight > p.Y)
                        {
                            ItemSelected(item, e);
                            return;
                        }
                        y += item.ActualHeight;
                    }
                };
                
                AllowsTransparency = true;
                Placement = Primitives.PlacementMode.Bottom;
            }
            public UIElementCollection Children => _itemsContent.Children;
            public event EventHandler ItemSelected;
        }

        public string Text
        {
            get => _textBox.Text;
            set => _textBox.Text = value;
        }
        public TextBox Editor => _textBox;
        public MyInputPopup Popup => _popup;
        new public bool Focus()
        {
            return _textBox.Focus();
        }

        public static readonly DependencyProperty ItemHeightProperty =
            DependencyProperty.Register("ItemHeight", typeof(double), typeof(MyTextBox));
        public double ItemHeight
        {
            get => (double)GetValue(ItemHeightProperty);
            set => SetValue(ItemHeightProperty, value);
        }
        public void SetOptions(IEnumerable<string> values)
        {
            _popup.Children.Clear();
            foreach (var text in values)
            {
                var btn = new MyInputPopupItem {
                    Text = text,
                };
                _popup.Children.Add(btn);
            }
        }

        public static readonly DependencyProperty IsEditingProperty =
            DependencyProperty.Register("IsEditing", typeof(bool), typeof(MyTextBox));

        protected MyInputPopup _popup;
        public void ShowPopup(bool? show)
        {
            bool b = show.HasValue ? show.Value : _popup.IsOpen ^ true;
            if (b)
            {
                if (_popup.Children.Count == 0) return;
                _popup.MinWidth = this.ActualWidth;
            }
            _popup.IsOpen = b;
        }

        public MyImage CreateButton(string name)
        {
            var img = this.GetImage(name);
            img.Stroke = this.BorderBrush;
            img.HorizontalAlignment = HorizontalAlignment.Right;

            Content.Add(img);
            return (MyImage)img;
        }

        protected TextBox _textBox;
        public MyTextBox()
        {
            Content.Add(_textBox = new TextBox { 
                BorderThickness = new Thickness(0),
            });
            Content.Add(_popup = new MyInputPopup {
                PlacementTarget = this,
            });

            _textBox.PreviewKeyDown += (s, e) => {
                switch (e.Key)
                {
                    case Key.LeftCtrl:
                    case Key.RightCtrl:
                    case Key.LeftAlt:
                    case Key.RightAlt:
                    case Key.LeftShift:
                    case Key.RightShift:
                        return;
                }

                KeyPressed?.Invoke(this, e);
            };
            _textBox.PreviewKeyUp += (s, e) => {
                if (e.Key == Key.F2)
                {
                    ShowPopup(null);
                }
            };

            var lineColor = this.BorderBrush.Clone();
            lineColor.Opacity = 0.5;
            this.BorderBrush = lineColor;

            _textBox.LostFocus += (s, e) => {

                var elem = Keyboard.FocusedElement;
                if (elem is MyInputPopupScroller)
                {

                }
                lineColor.Opacity = 0.5;

                SetValue(IsEditingProperty, false);
                ShowPopup(false);
            };
            _textBox.GotFocus += (s, e) => {
                lineColor.Opacity = 1;
            };
            _popup.ItemSelected += (s, e) => {
                Text = ((MyInputPopupItem)s).Text;
            };
        }
        public event EventHandler<KeyEventArgs> KeyPressed;
    }
    public partial class TextInput : MyEditor
    {
        protected virtual bool IsKeyValid(char key)
        {
            return true;
        }
        protected char GetLastChar()
        {
            var text = ((MyTextBox)Editor).Text;
            if (text == string.Empty) return ' ';

            return text[text.Length - 1];
        }
        protected bool EndWithNumber()
        {
            var c = GetLastChar();
            return c >= '0' && c <= '9';
        }
        protected virtual void OnEditorLossFocus() { }
        protected override FrameworkElement CreateEditorCore()
        {
            var text = new MyTextBox();
            text.KeyPressed += (s, e) => {
                if (IsNumber(e.Key)) return;

                var c = e.Key.ToChar();
                if (c >= ' ')
                {
                    e.Handled = !IsKeyValid(c);
                }
            };
            text.Editor.LostFocus += (s, e) => {
                OnEditorLossFocus();
            };

            return text;
        }
        protected override object GetValueCore() => ((MyTextBox)Editor).Text.Trim();
        protected override void SetValueCore(object value) { ((MyTextBox)Editor).Text = value?.ToString(); }
        protected bool IsNumber(Key key)
        {
            return (key >= Key.NumPad0 && key <= Key.NumPad9) || (key >= Key.D0 && key <= Key.D9);
        }
        public override void SetOptions(string value)
        {
            if (string.IsNullOrEmpty(value)) return;
            this.SetOptionsCore(value);
        }
        protected virtual void SetOptionsCore(string value)
        {
            ((MyTextBox)Editor).SetOptions(value.Split(';'));
        }
    }
    public class FileInput : TextInput
    {
        MyImage _button;
        string _filter;
        public FileInput()
        {
            var textBox = (MyTextBox)this.Editor;
            _button = textBox.CreateButton("File");
            _button.Cursor = Cursors.Hand;

            _button.ApplyMouseClickAction(() => {
                var dlg = new Microsoft.Win32.OpenFileDialog {
                    Filter = _filter,
                };
                if (dlg.ShowDialog() == true) {
                    this.Value = dlg.FileName;
                }
            });
        }

        protected override void SetOptionsCore(string value)
        {
            _filter = value;
        }
        protected override Size MeasureOverride(Size constraint)
        {
            Editor.Measure(constraint);
            _button.Width = _button.Height = Editor.DesiredSize.Height / 2;

            return base.MeasureOverride(constraint);
        }

    }
    public class DateInput : TextInput
    {
        protected virtual int[] GetTimeItems(int len, params char[] sep)
        {
            var items = ((string)base.GetValueCore()).Split(sep);
            var v = new int[len];

            for (int i = 0; i < items.Length && i < len; i++)
            {
                int a = 0;
                int.TryParse(items[i], out a);

                v[i] = a;
            }
            return v;
        }
        protected override bool IsKeyValid(char key)
        {
            switch (key)
            {
                case ' ':
                case '/':
                case '.':
                case '-':
                    return EndWithNumber();
            }
            return false;
        }
        protected override void SetValueCore(object value)
        {
            if (value is DateTime)
            {
                value = string.Format("{0:dd/MM/yyyy}", value);
            }
            base.SetValueCore(value);
        }
        protected override object GetValueCore()
        {
            var v = GetTimeItems(3, ' ', '/', '-', '.');
            int d = v[0];
            if (d == 0) return null;

            var today = DateTime.Today;
            int m = v[1];
            if (m == 0) m = today.Month;

            int y = v[2];
            if (y == 0) y = today.Year;

            return new DateTime(y, m, d);
        }
        protected override void OnEditorLossFocus()
        {
            SetValueCore(GetValueCore());
        }
    }
    public class TimeInput : DateInput
    {
        protected override bool IsKeyValid(char key)
        {
            switch (key)
            {
                case ' ':
                case ':':
                    return EndWithNumber();
            }
            return false;
        }
        protected override void SetValueCore(object value)
        {
            if (value is DateTime)
            {
                value = string.Format("{0:HH:mm}", value);
            }
            base.SetValueCore(value);
        }
        protected override object GetValueCore()
        {
            var v = GetTimeItems(2, ' ', ':');
            var d = DateTime.Today;

            var h = Math.Min(23, v[0]);
            var m = Math.Min(59, v[1]);

            return new DateTime(d.Year, d.Month, d.Day, h, m, 0);
        }
    }
    public class NumberInput : TextInput
    {
        protected override bool IsKeyValid(char key)
        {
            switch (key)
            {
                case '+':
                case '-':
                    return GetLastChar() == ' ';

                case '.':
                    var text = (string)base.GetValueCore();
                    if (text == string.Empty || text.IndexOf('.') > 0)
                    {
                        return false;
                    }
                    return EndWithNumber();
            }
            return false;
        }
    }
    public class ComboInput : TextInput
    {
        MyImage _down;
        protected override FrameworkElement CreateEditorCore()
        {
            var text = (MyTextBox)base.CreateEditorCore();

            text.Editor.IsReadOnly = true;
            text.Editor.Cursor = Cursors.Hand;
            _down = text.CreateButton("Down");
            this.PreviewMouseLeftButtonDown += (s, e) => {
                text.ShowPopup(null);
            };

            return text;
        }
        protected override Size MeasureOverride(Size constraint)
        {
            Editor.Measure(constraint);
            _down.Width = _down.Height = Editor.DesiredSize.Height / 2;

            return base.MeasureOverride(constraint);
        }
    }
}
