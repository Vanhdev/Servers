using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace ClientDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        #region Messaging
        void SendConnect()
        {
            _mqtt.Publish(null, "upload/connect", new { model = "SECURITY", version = "V30" });
        }
        Device _device = new Device();
        public string FormatStatus()
        {
            string s = string.Empty;
            byte b = _device.Status[0];
            _device.Status[0] = (byte)(b >> 2);

            foreach (var v in _device.Status)
            {
                s += string.Format("{0:x2}", v);
            }
            _device.Status[0] = b;
            return s;
        }
        void SendStatus()
        {
            _mqtt.Publish(null, "upload/status", FormatStatus());
        }
        void GetTime()
        {
            _mqtt.Publish(null, "request/getTime", null);
        }
        #endregion

        Led[] leds;
        Button[] buttons;
        
        Thread _blinker;
        Mqtt _mqtt;

        public MainWindow()
        {
            InitializeComponent();

            var leds = new List<Led>();
            var buttons = new List<Button>();

            string[] names = new string[] {
                "ARM 1", "ARM 2", "DISARM", "CLA",
                "INPUT 0", "INPUT 1", "INPUT 2", "INPUT 3",
                "ZONE 0", "ZONE 1", "ZONE 2", "ZONE 3",
            };

            int i = 0;
            foreach (var child in ((Grid)Content).Children)
            {
                if (child is Led)
                {
                    var led = (Led)child;
                    if (leds.Count >= 8)
                    {
                        led.Color = Brushes.Red;
                    }

                    led.SetValue(Grid.ColumnSpanProperty, 2);
                    leds.Add(led);
                    continue;
                }

                if (child is Button)
                {
                    var btn = (Button)child;
                    var name = names[i++];
                    btn.Content = name;
                    btn.SetValue(Grid.ColumnSpanProperty, 2);
                    btn.Margin = new Thickness(2);

                    btn.Click += (s, e) => {
                        var vs = name.Split(' ');
                        Demo(vs[0], vs.Length < 2 ? -1 : vs[1][0] & 15);
                    };
                    if (buttons.Count >= 4)
                    {
                        btn.BorderBrush = Brushes.Transparent;
                    }
                    buttons.Add(btn);
                }
            }

            this.leds = leds.ToArray();
            this.buttons = buttons.ToArray();

            this.Closing += (s, e) =>
            {
                try
                {
                    _blinker.Abort();
                    _mqtt.Client.Disconnect();
                }
                catch
                {
                }
            };

            _mqtt = new Mqtt();
            _mqtt.ConnectionChanged += (s, e) => {
                if (_mqtt.IsConnected)
                {
                    _device.Status[0] |= 2;
                    SendConnect();
                }
                else
                {
                    _device.Status[0] &= 0xdf;
                }
                UpdateLeds(_device.Status[0], 0, 2);
            };

            var lines = new ScreenLine();

            _mqtt.MessageReceived += (s, e) => {
                var command = _mqtt.Response.GetValue<string>();
                screen.Dispatcher.InvokeAsync(() => {
                    lines.Add(command);
                    screen.Text = lines.ToString();
                });

                try
                {
                    if (_device.Run(command))
                    {
                        Dispatcher.InvokeAsync(() => ShowIndicators());
                    }
                }
                catch
                {
                }
            };

            _mqtt.Connect(null);

            _blinker = new Thread(new ThreadStart(() => {
                int minute = 60;
                while (true)
                {
                    UpdateLeds(_device.Status[1], 4, 4);
                    Thread.Sleep(500);

                    UpdateLeds(0, 4, 4);
                    Thread.Sleep(500);

                    if (--minute == 0)
                    {
                        minute = 60;

                        _mqtt.Connect(null);
                        //GetTime();
                    }
                }
            }));
            _blinker.Start();

            Demo("ARM", 1);
        }
        void Demo(string name, int index)
        {
            if (name == "DISARM")
            {
                name = "ARM";
                index = 0;
            }
            _device.Demo(name, index);
            ShowIndicators();
        }
        public void ShowIndicators(bool send)
        {
            _device.Status[0] |= 1;
            _device.Status[0] |= (byte)(_mqtt.IsConnected ? 2 : 0);

            UpdateLeds(_device.Status[0], 0, 4);
            UpdateLeds(_device.Status[1], 8, 4);
            UpdateButtons(_device.Status[2], 4);
            UpdateButtons(_device.Status[3], 8);

            if (send)
            {
                SendStatus();
            }
        }
        public void ShowIndicators()
        {
            this.ShowIndicators(true);
        }

        public void UpdateLeds(int value, int start, int length)
        {
            Dispatcher.InvokeAsync(() => {
                for (int i = 0; i < length; i++)
                {
                    leds[start + i].Value = value & (1 << i);
                }
            });
        }
        public void UpdateButtons(int value, int start)
        {
            Dispatcher.InvokeAsync(() => {
                for (int i = 0; i < 4; i++)
                {
                    var btn = buttons[start + i];
                    if ((value & (1 << i)) == 0)
                    {
                        btn.BorderBrush = Brushes.Transparent;
                    }
                    else
                    {
                        btn.BorderBrush = Brushes.Red;
                    }
                }
            });
        }
    }
}
