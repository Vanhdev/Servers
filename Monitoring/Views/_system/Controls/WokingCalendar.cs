using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using VST.WPF;

namespace System.Windows.Controls
{
    public class WeekDay : Grid
    {
        public WeekDay()
        {
            this.Background = Brushes.WhiteSmoke;
            this.Margin = new Thickness(1, 1, 0, 0);
            Children.Add(new TextBlock
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            });
        }
        public TextBlock Caption => (TextBlock)Children[0];
        public string Text
        {
            get => Caption.Text;
            set => Caption.Text = value;
        }
    }
    public class WorkingDay : WeekDay
    {

    }
    public partial class WorkingCalendar : MyUserControl<StackPanel>
    {
        #region Items Controls
        public class Week<T> : Grid
            where T : WeekDay, new()
        {
            protected WorkingCalendar info;
            public WeekDay Caption { get; private set; }
            public Week(WorkingCalendar calendar)
            {
                this.info = calendar;
                this.Add(Caption = new WeekDay());
                for (int i = 0; i < 7; i++)
                {
                    this.Add(new T(), 0, i + 1);
                }
            }
            public T this[int index]
            {
                get => (T)Children[index + 1];
            }
            protected override void OnRender(DrawingContext dc)
            {
                base.OnRender(dc);
                for (int i = 0; i < 5; i++)
                {
                    this[i].Style = info.DayStyle;
                }
                this[5].Style = info.SunDayStyle;
                this[6].Style = info.SunDayStyle;
                this.ColumnDefinitions[0].Width = new GridLength(info.RowHeaderWidth);
            }
        }
        public class Working : Week<WorkingDay>
        {
            public DateTime Start { get; private set; }
            public DateTime End => Start.AddDays(6);
            public Working(ref DateTime date, WorkingCalendar info) : base(info) 
            {
                Start = date;
                this.Caption.Text = string.Format("{0:dd.MM.yyyy}", date);
                for (int i = 0; i < 7; i++)
                {
                    this[i].Text = date.Day.ToString();
                    date = date.AddDays(1);
                }
            }

            protected override void OnRender(DrawingContext dc)
            {
                base.OnRender(dc);
                for (int i = 0; i < 7; i++)
                {
                    var cap = this[i].Caption;
                    cap.FontSize = this.ActualWidth / 20;
                    cap.Opacity = 0.1;
                }
            }
        }
        #endregion

        #region Apperance
        public TextStyle SunDayStyle { get; set; } = new TextStyle();
        public TextStyle DayStyle { get; set; } = new TextStyle();
        public double RowHeaderHeight { get; set; } = 40;
        public double RowHeaderWidth { get; set; } = 120;
        #endregion

        Week<WeekDay> _header;
        StackPanel _main_content;
        MyMenuBar _menu;
        public MyMenuBar Menu => _menu;
        public WorkingCalendar()
        {

            var grid = new Grid();

            Background = Brushes.Gray;

            grid.Add(_header = new Week<WeekDay>(this));
            grid.Add(_main_content = new StackPanel(), 1, 0);

            var days = new string[] { "T2", "T3", "T4", "T5", "T6", "T7", "CN" };
            for (int i = 0; i < 7; i++)
            {
                _header[i].Text = days[i];
                _header[i].Caption.FontWeight = FontWeights.Bold;
            }
            SunDayStyle.Add(TextBlock.ForegroundProperty, (Brush)RGB.Parse("#c00"));

            this.Content.Children.Add(_menu = new MyMenuBar {
            });
            this.Content.Children.Add(grid);
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);
            int d = e.Delta;
            if (d < 0)
            {
                SetFirstVisible(_firstVisible + 1);
            }
            else
            {
                SetFirstVisible(_firstVisible - 1);
            }
        }

        public void Render(DateTime start, int weeks, Action<DateTime, WorkingDay> dayCreatedCallback)
        {
            var day = (int)start.DayOfWeek;
            if (day == 0) day = 7;

            var date = start.AddDays(1 - day);

            _main_content.Children.Clear();
            _firstVisible = 0;

            for (int i = 0; i < weeks; i++)
            {
                start = date;

                var row = new Working(ref date, this);
                _main_content.Children.Add(row);

                for (int d = 0; d < 7; d++)
                {
                    dayCreatedCallback.Invoke(start.AddDays(d), row[d]);
                }
            }
        }

        int _firstVisible;
        double _rowHeight;
        public void SetFirstVisible(int index)
        {
            if (index < 0) { return; }

            _firstVisible = index;
            InvalidateArrange();
        }

        protected override Size MeasureOverride(Size constraint)
        {
            ((Grid)Content.Children[1]).RowDefinitions[0].Height = new GridLength(RowHeaderHeight);

            _rowHeight = (constraint.Width - RowHeaderWidth) / 7;
            foreach (Working row in _main_content.Children)
            {
                row.Height = _rowHeight;
            }

            return base.MeasureOverride(constraint);
        }
        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            int i = 0;
            var rect = new Rect(0, 0, arrangeBounds.Width, _rowHeight);
            foreach (Working row in _main_content.Children)
            {
                if (i++ < _firstVisible)
                {
                    row.Visibility = Visibility.Hidden;
                    continue;
                }

                row.Arrange(rect);
                row.Visibility = Visibility.Visible;
                rect.Y += _rowHeight;

                if (rect.Y > arrangeBounds.Height) break;
            }
            return base.ArrangeOverride(arrangeBounds);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {

        }
    }
}
