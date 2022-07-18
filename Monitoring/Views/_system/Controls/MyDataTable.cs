using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Vst.GUI;

namespace System
{
    public class ValueFilter
    {
        protected object value;
        protected virtual bool IsMatchCore(object comparedValue, object filteringValue)
        {
            return comparedValue.Equals(filteringValue);
        }
        public Func<object, object, bool> Condition { get; set; }
        public ValueFilter()
        {
            Condition = IsMatchCore;
        }
        public ValueFilter(Func<object, object, bool> condition)
        {
            Condition = condition;
        }
        public bool IsMacth(object filteringValue)
        {
            return Condition(value, filteringValue);
        }
        public virtual void SetValue(object filterValue)
        {
            value = filterValue ?? string.Empty;
        }
    }
}

namespace System.Windows.Controls
{
    public static class VisualExtension
    {
        //public static double PixelsPerDip(this Visual view)
        //{
        //    return VisualTreeHelper.GetDpi(view).PixelsPerDip;
        //}
    }

    public abstract class DataControl : UserControl
    {
        protected Status _status = new Status();
        public DataTable Table { get; set; }

        public DataControl(DataTable table)
        {
            Table = table;
        }
    }

    public class FilterTextBox : MyUserControl
    {
        public FilterTextBox(DataTable table, string name, string comment) { }
    }

}

namespace System.Windows.Controls
{
    public static class GridExtension
    {
        public static UIElement Add(this Grid g, UIElement e, int row, int col)
        {
            for (int i = g.RowDefinitions.Count; row >= i; i++)
                g.RowDefinitions.Add(new RowDefinition());
            for (int i = g.ColumnDefinitions.Count; col >= i; i++)
                g.ColumnDefinitions.Add(new ColumnDefinition());

            e.SetValue(Grid.RowProperty, row);
            e.SetValue(Grid.ColumnProperty, col);
            g.Children.Add(e);

            return e;
        }
        public static UIElement Add(this Grid g, UIElement e)
        {
            g.Children.Add(e);
            return e;
        }
    }
    public partial class DataTable : MyUserControl
    {
        #region Sub classes
        public DataTableColumnCollection Columns { get; private set; } = new DataTableColumnCollection();
        #endregion

        public class ColumnSeeker : MyImageButton
        {
            public ColumnSeeker(string name)
            {
                ImageSource = name;
                var border = new Border
                {
                    CornerRadius = new CornerRadius(Width / 2),
                    Background = this.Background,
                };
                Background = null;

                HorizontalAlignment = (name[0] == 'R' ?
                    HorizontalAlignment.Right : HorizontalAlignment.Left);

                this.Content.Add(border);
                this.Visibility = Visibility.Collapsed;
            }
            new public bool IsVisible
            {
                get => base.IsEnabled;
                set => Visibility = value ? Visibility.Visible : Visibility.Collapsed;
            }
            public int Value { get; set; }
        }

        ColumnSeeker _rightSeeker;
        ColumnSeeker _leftSeeker;

        Panel _rowsContent;
        DataTableHeader _header;
        DataTableRowIndexColumn _rowIndexColumn;
        DataTableRowSelectColumn _rowCheck;
        public DataTableHeader Header => _header;
        public bool IsRowIndexVisible
        {
            get => _rowIndexColumn.IsVisible;
            set => _rowIndexColumn.IsVisible = value;
        }
        public bool IsRowCheckVisible
        {
            get => _rowCheck.IsVisible;
            set => _rowCheck.IsVisible = value;
        }

        DataTableColumn[] _dataColumns;
        public DataTableColumn[] DataColumns
        {
            get
            {
                if (_dataColumns == null)
                {
                    var lst = new List<DataTableColumn>();

                    foreach (var col in Columns)
                    {
                        if (!string.IsNullOrEmpty(col.Name))
                        {
                            lst.Add(col);
                        }
                    }
                    _dataColumns = lst.ToArray();
                }
                return _dataColumns;
            }
        }

        public DataTable()
        {
            this.Content.Add(_header = new DataTableHeader(this));
            this.Content.Add(_rowsContent = new StackPanel
            {
                VerticalAlignment = VerticalAlignment.Top,
            }, 1, 0);
            Content.Add(_rightSeeker = new ColumnSeeker("Right"), 1, 0);
            Content.Add(_leftSeeker = new ColumnSeeker("Left"), 1, 0);


            this.Content.RowDefinitions[0].Height = GridLength.Auto;

            Focusable = true;
            IsHeaderVisible = true;

            Columns.Add(_rowIndexColumn = new DataTableRowIndexColumn());
            Columns.Add(_rowCheck = new DataTableRowSelectColumn());

            _rightSeeker.Click += (s, e) =>
            {
                if (_leftSeeker.Value < DataColumns.Length - 1)
                {
                    DataColumns[_leftSeeker.Value++].IsVisible = false;
                    InvalidateMeasure();
                }
            };
            _leftSeeker.Click += (s, e) =>
            {
                if (_leftSeeker.Value > 0)
                {
                    for (int i = --_leftSeeker.Value; i < DataColumns.Length; i++)
                    {
                        DataColumns[i].IsVisible = true;
                    }
                    InvalidateMeasure();
                }
            };
        }

        #region SIZE
        public static readonly DependencyProperty RowHeightProperty =
            DependencyProperty.Register("RowHeight", typeof(double), typeof(DataTable),
                new PropertyMetadata(40.0));
        public double RowHeight
        {
            get => (double)GetValue(RowHeightProperty);
            set => SetValue(RowHeightProperty, value);
        }
        #endregion

        #region EVENTS
        void OnRowFocused(DataTableRow row, int index)
        {
            if (_focused != null) _focused.SetFocus(false);
            _focused = row;
        }
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                SetCurrentIndex(FocusedIndex + 1);
                e.Handled = true;
                return;
            }
            if (e.Key == Key.Up)
            {
                SetCurrentIndex(FocusedIndex - 1);
                e.Handled = true;
                return;
            }
            base.OnKeyDown(e);
        }
        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
                if (_focused != null)
                {
                    _focused.IsChecked ^= true;
                }
                return;
            }
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                RaiseOpenItem();
                return;
            }
            base.OnKeyUp(e);
        }
        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            int d = e.Delta < 0 ? 1 : -1;
            SetFirstVisibleOffset(d);
        }
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            Focus();
        }
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            if (e.ClickCount > 1)
            {
                RaiseOpenItem();
            }
        }
        public void RaiseOpenItem()
        {
            if (_focused != null && OpenItem != null)
            {
                OpenItem(_focused.DataContext);
            }
        }
        public void RaiseItemFocused()
        {
            if (_focused != null && ItemFocused != null)
            {
                ItemFocused(_focused.DataContext);
            }
        }
        public Action<object> OpenItem;
        public Action<object> ItemFocused;
        #endregion

        #region PROPERTIES
        bool _isHeaderVisible = true;
        public bool IsHeaderVisible
        {
            get => _isHeaderVisible;
            set
            {
                if (IsHeaderVisible != value)
                {
                    _header.Visibility = value ? Visibility.Visible : Visibility.Hidden;
                    InvalidateMeasure();
                }
            }
        }
        public int FocusedIndex
        {
            get
            {
                if (_focused == null) return -1;
                return _focused.Index;
            }
        }
        #endregion

        #region ROWS
        DataTableRow[] _filteredRows = new DataTableRow[] { };
        protected List<DataTableRow> _rows = new List<DataTableRow>();

        DataTableRow _focused;
        public DataTableRow FocusedRow
        {
            get => _focused;
            set
            {
                if (_focused != value)
                {
                    _focused = value;
                    if (value != null)
                    {
                        ItemFocused?.Invoke(_focused.DataContext);
                    }
                }
            }
        }

        int _firstVisible;
        public int FirstVisible => _firstVisible;

        int _visibleRows;

        protected void CreateRows()
        {
            _rows = new List<DataTableRow>();
            _rowsContent.Children.Clear();

            _header.IsChecked = false;

            if (_items != null)
            {
                int i = 0;
                foreach (var e in _items)
                {
                    var row = CreateRowCore(i++);
                    row.DataContext = e;

                    _rows.Add(row);
                }
            }

            _filteredRows = _rows.ToArray();
        }
        protected virtual DataTableRow CreateRowCore(int index)
        {
            var row = new DataTableRow(this, index);
            row.GotFocus += (s, e) => OnRowFocused(row, index);


            return row;
        }
        public void SetFirstVisibleOffset(int d)
        {
            SetFirstVisible(_firstVisible + d);
        }
        public void SetFirstVisible(int index)
        {
            if (index >= _filteredRows.Length) index = _filteredRows.Length - 1;
            if (index < 0) index = 0;

            if (_firstVisible != index)
            {
                _firstVisible = index;
                InvalidateMeasure();
            }
        }
        void SetCurrentIndex(int index)
        {
            if (index == FocusedIndex) return;
            if (index >= 0 && index < _filteredRows.Length)
            {
                var row = _filteredRows[index];
                row.SetFocus(true);
                _focused?.SetFocus(false);
                _focused = row;

                if (index < _firstVisible)
                {
                    SetFirstVisible(index);
                }
                else if (index >= _firstVisible + _visibleRows)
                {
                    SetFirstVisible(index - _visibleRows + 1);
                }

                RaiseItemFocused();
            }
        }
        public void OnDataTableRowCheckedChanged(bool b) { }
        public void SelectAll(bool b)
        {
            Header.IsChecked = b;
            foreach (var row in _filteredRows)
            {
                row.IsChecked = b;
            }
        }
        public DataTableRow[] DisplayedRows => _filteredRows;
        public DataTableRow[] SelectedRows
        {
            get
            {
                var lst = new List<DataTableRow>();
                foreach (var row in _filteredRows)
                {
                    if (row.IsChecked)
                    {
                        lst.Add(row);
                    }
                }
                return lst.ToArray();
            }
        }
        public void RemoveSelectedRows()
        {
            foreach (var row in SelectedRows)
            {
                _rows.Remove(row);
                _filteredRows = _rows.ToArray();

                _firstVisible = -1;
                SetFirstVisible(0);
            }
        }
        public object[] SelectedItems
        {
            get
            {
                var rows = SelectedRows;
                var items = new object[rows.Length];
                for (int i = 0; i < rows.Length; i++)
                {
                    items[i] = rows[i].DataContext;
                }
                return items;
            }
        }
        public object SelectedItem
        {
            get => _focused?.DataContext;
        }
        #endregion

        #region DATA

        IEnumerable _items;
        public IEnumerable ItemsSource
        {
            get => _items;
            set
            {
                _firstVisible = 0;
                _focused = null;
                _items = value;
                CreateRows();
                InvalidateMeasure();
            }
        }
        public virtual object[] GetDisplayItemValue(object item)
        {
            var lst = new List<object>();
            var props = item.GetType();
            foreach (DataTableColumn col in DataColumns)
            {
                if (col.IsVisible == false) continue;

                object v = null;
                v = props.GetProperty(col.Name)?.GetValue(item);
                lst.Add(v);
            }
            return lst.ToArray();
        }
        public string Copy()
        {
            var header = new List<string>();
            foreach (var col in Columns)
            {
                if (col.Name != null && col.IsVisible)
                {
                    header.Add(col.Header);
                }
            }
            var s = string.Join("\t", header);
            foreach (var row in _rows)
            {
                s += '\n';
                s += string.Join("\t", GetDisplayItemValue(row.DataContext));
            }
            return s;
        }
        #endregion

        #region FILTER
        public void ClearFilter()
        {
            _filteredRows = _rows.ToArray();
            _firstVisible = -1;
            SetFirstVisible(0);
        }
        public void ExecuteFilter()
        {
            var lst = new List<DataTableRow>();
            foreach (DataTableRow row in _rows)
            {
                var item = row.DataContext;
                var props = item.GetType();

                bool match = true;
                foreach (var col in Columns)
                {
                    //if (col.Filter == null) continue;

                    var p = props.GetProperty(col.Name);
                    if (p == null) continue;

                    var v = p.GetValue(item) ?? string.Empty;
                    //match = col.Filter.IsMacth(v);

                    if (!match) break;
                }
                if (match) lst.Add(row);
            }

            _filteredRows = lst.ToArray();
            _firstVisible = -1;

            SetFirstVisible(0);
        }
        #endregion

        #region CALCULATE
        public void Invalidate()
        {
            if (IsHeaderVisible)
            {
                _header.InvalidateMeasure();
            }
            this.InvalidateMeasure();
        }
        protected override Size MeasureOverride(Size constraint)
        {
            double w = 0;

            _leftSeeker.Value = 0;
            //_rightSeeker.IsVisible = false;

            foreach (var col in Columns)
            {
                if (col.IsVisible)
                {
                    if (w > constraint.Width)
                    {
                        break;
                    }
                    w += col.ColumnInfo.Width ?? 100;
                }
                else
                {
                    if (col.Name != null)
                    {
                        _leftSeeker.Value++;
                        _leftSeeker.IsVisible = true;
                    }
                }
            }

            if (w > constraint.Width)
            {
                _rightSeeker.IsVisible = true;
            }

            w = constraint.Width;
            double y = 0;

            if (IsHeaderVisible)
            {
                y = _header.Height;
                _header.Width = w;
            }

            _rowsContent.Children.Clear();
            if (_filteredRows.Length > 0)
            {
                double visibleHeight = constraint.Height;
                double h = RowHeight;

                _visibleRows = 0;
                for (int i = _firstVisible; i < _filteredRows.Length; i++)
                {
                    var row = _filteredRows[i];
                    row.Width = w;
                    row.Height = h;

                    row.Measure(constraint);

                    _rowsContent.Children.Add(row);

                    y += h;
                    if (y >= visibleHeight) break;

                    _visibleRows++;
                }
            }

            return base.MeasureOverride(new Size(w, y));
        }
        #endregion
    }
}

#region DataTableRowBase
namespace System.Windows.Controls
{
    public class RowIndicator : MyUserControl
    {
        public void SetActive(bool active)
        {
            Content.Background = active ? Foreground : Brushes.Transparent;
        }
    }
    public abstract class DataTableRowBase : MyUserControl
    {
        protected Status _status = new Status();
        protected RowIndicator _indicator;
        protected StackPanel _cellContent;
        public DataTable Table { get; private set; }
        public DataTableRowBase(DataTable table)
        {
            Table = table;
        }
        public UIElementCollection Cells => _cellContent.Children;
        public DataTableCell this[int index] => (DataTableCell)Cells[index];

        protected abstract DataTableCell CreateCellCore(DataTableColumn column);
        protected void ForEachCell(Action<DataTableCell, DataTableColumn> invoke)
        {
            var col = Table.Columns.First;
            foreach (DataTableCell cell in Cells)
            {
                if (col.Value.IsVisible)
                {
                    invoke(cell, col.Value);
                    cell.Width = col.Value.Width;
                    cell.Visibility = Visibility.Visible;
                }
                else
                {
                    cell.Visibility = Visibility.Collapsed;
                }
                col = col.Next;
            }
        }

        protected const int _focused = 0;
        protected const int _hover = 1;
        protected const int _checked = 2;
        public bool IsChecked
        {
            get => _status[_checked];
            set
            {
                if (IsChecked == value) return;

                _status[_checked] = value;
                if (IsVisible)
                {
                    this[1].DisplayValue = value;
                    InvalidateVisual();
                }
            }
        }

        public virtual void SetFocus(bool value)
        {
            _status[_focused] = value;
            _indicator.SetActive(value);
        }
        protected virtual void OnSelectManually(bool isChecked) { }
        protected override Size MeasureOverride(Size constraint)
        {
            if (_cellContent == null)
            {
                _indicator = new RowIndicator();
                _cellContent = new StackPanel
                {
                    Orientation = Orientation.Horizontal
                };
                _status.Changed += () => InvalidateVisual();

                Content.Add(_indicator);
                Content.Add(_cellContent, 0, 1);

                Content.ColumnDefinitions[0].Width = GridLength.Auto;

                foreach (var col in Table.Columns)
                {
                    Cells.Add(CreateCellCore(col));
                }

                if (Table.IsRowCheckVisible)
                {
                    var chk = this[1] as DataTableCheckCell;
                    if (chk != null)
                    {
                        chk.Displayer.IsCheckedChanged += (s, e) =>
                        {
                            var b = (bool)chk.DisplayValue;

                            _status[_checked] = b;
                            OnSelectManually(b);
                        };
                    }
                }
            }
            ForEachCell((cell, col) => { });

            return base.MeasureOverride(constraint);
        }
    }
}
#endregion

#region DataTableRow
namespace System.Windows.Controls
{
    partial class DataTableColumn
    {
        protected class RowIndexHeaderCell : DataTableHeaderCell
        {
            public RowIndexHeaderCell(DataTableHeader row, DataTableColumn column)
                : base(column)
            {

            }
        }
    }
    public class DataTableRowIndexColumn : DataTableTextColumn
    {
        //protected class LeftTopCorner : DataTableHeaderCell
        //{
        //    public LeftTopCorner(DataTableRowBase row, DataTableColumn col)
        //        : base(row, col)
        //    {
        //        Padding = new Thickness(0);
        //        //((TextBlock)((Grid)Content).Children[0]).Text = new string(' ', 50);
        //    }
        //}
        //protected override DataTableCell CreateHeaderCellCore(DataTableHeader row)
        //{
        //    return new LeftTopCorner(row, this);
        //}

        public DataTableRowIndexColumn()
        {
            ColumnInfo.Width = 40;
            ColumnInfo.TextAlignment = 2;
        }
        public class RowIndexCell : DataTableTextCell
        {
            public RowIndexCell(DataTableColumn col) : base(col)
            {
                HorizontalAlignment = HorizontalAlignment.Right;
            }
        }
        public override DataTableCell CreateDataCell()
        {
            return new RowIndexCell(this);
        }
    }
    public class DataTableRowSelectColumn : DataTableCheckColumn
    {
        public DataTableRowSelectColumn()
        {
            Width = 32;
            Resizable = false;
        }
        protected override DataTableCell CreateHeaderCell()
        {
            return new DataTableCheckCell();
        }
    }
    public class DataTableRow : DataTableRowBase
    {
        public int Index { get; private set; }
        public DataTableRow(DataTable table, int index) : base(table)
        {
            Index = index;
        }
        protected override DataTableCell CreateCellCore(DataTableColumn column)
        {
            return column.CreateDataCell();
        }
        protected override Size MeasureOverride(Size constraint)
        {
            var size = base.MeasureOverride(constraint);
            RenderData();

            return size;
        }
        protected virtual void RenderData()
        {
            if (DataContext == null) { return; }
            var values = Table.GetDisplayItemValue(DataContext);

            if (Table.IsRowCheckVisible || Table.IsRowIndexVisible)
            {
                var lst = new List<object>();
                lst.AddRange(values);
                if (Table.IsRowCheckVisible) lst.Insert(0, IsChecked);
                if (Table.IsRowIndexVisible) lst.Insert(0, Index + 1);

                values = lst.ToArray();
            }

            int i = 0;
            ForEachCell((cell, col) => cell.DisplayValue = col.GetDisplayValue(values[i++]));
        }
        protected override void OnSelectManually(bool b)
        {
            foreach (var row in Table.DisplayedRows)
            {
                if (row.IsChecked != b)
                {
                    b = false;
                    break;
                }
            }
            Table.Header.IsChecked = b;
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            _status[_hover] = true;
        }
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            _status[_hover] = false;
        }
        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            base.OnMouseDoubleClick(e);
            Table.OpenItem?.Invoke(DataContext);
        }
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            if (Table.FocusedRow != this)
            {
                Table.FocusedRow?.SetFocus(false);
                SetFocus(true);

                Table.Focus();
                Table.FocusedRow = this;
            }
        }
    }
}
#endregion

#region Header
namespace System.Windows.Controls
{
    public class DataTableHeaderCell : DataTableTextCell
    {
        static bool beginResize;

        DataTableColumn _column;
        public DataTableHeaderCell(DataTableColumn column)
            : base(column)
        {
            _column = column;

            Content.Children.Add(new TextBlock
            {
                Text = column.Header,
                HorizontalAlignment = column.TextAlignment,
            });
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            var p = e.GetPosition(this);

            if (IsMouseCaptured)
            {
                _column.Width = p.X;
                return;
            }

            beginResize = _column.Resizable && (p.X >= _column.Width - 5);
            if (beginResize)
            {
                Cursor = Cursors.SizeWE;
            }
            else
            {
                Cursor = Cursors.Arrow;
            }
        }
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            if (beginResize)
            {
                this.CaptureMouse();
            }
        }
        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            if (IsMouseCaptured)
            {
                this.ReleaseMouseCapture();
            }
        }
        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

        }
    }
    public class DataTableHeader : DataTableRowBase
    {
        public DataTableHeader(DataTable table)
            : base(table)
        {
            Height = table.RowHeight;
        }
        protected override void OnSelectManually(bool b)
        {
            Table.SelectAll(b);
        }
        protected override DataTableCell CreateCellCore(DataTableColumn column)
        {
            column.SizeChanged += (s, e) => Table.Invalidate();
            column.VisibleChanged += (s, e) => Table.Invalidate();
            return column.HeaderCell;
        }
    }
}
#endregion

#region Cells
public abstract class DataTableCell : MyUserControl
{
    public abstract object DisplayValue { get; set; }
}
public abstract class DataTableCell<T> : DataTableCell
    where T : UIElement, new()
{
    public DataTableCell()
    {
        this.Content.Add(new T());
    }
    public T Displayer => (T)Content.Children[0];
}
public class DataTableTextCell : DataTableCell<TextBlock>
{
    public DataTableTextCell(DataTableColumn column)
    {
        var tb = Displayer;
        tb.HorizontalAlignment = column.TextAlignment;

        var br = column.Background;
        if (br != null) tb.Background = Background;

        var color = column.Foreground;
        if (color != null) tb.Foreground = color;

    }
    public override object DisplayValue { get => Displayer.Text; set => Displayer.Text = (string)value; }
}
public class DataTableCheckCell : DataTableCell<MyCheckBox>
{
    public DataTableCheckCell()
    {
        this.HorizontalContentAlignment = HorizontalAlignment.Center;
    }
    public override object DisplayValue { get => Displayer.IsChecked; set => Displayer.IsChecked = (bool)value; }
}
#endregion

#region Columns
namespace System.Windows.Controls
{

    public abstract partial class DataTableColumn
    {
        public ColumnInfo ColumnInfo { get; set; } = new ColumnInfo();
        public string Header
        {
            get => ColumnInfo.Caption;
            set => ColumnInfo.Caption = value;
        }
        public string Name
        {
            get => ColumnInfo.Name;
            set => ColumnInfo.Name = value;
        }
        public HorizontalAlignment TextAlignment => (HorizontalAlignment)(ColumnInfo.TextAlignment ?? 1);

        public Brush Background { get; set; }
        public Brush Foreground { get; set; }
        public event EventHandler VisibleChanged;
        public bool IsVisible
        {
            get => ColumnInfo.IsVisible ?? true;
            set
            {
                if (value != ColumnInfo.IsVisible)
                {
                    ColumnInfo.IsVisible = value;
                    VisibleChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }
        public double MinWidth { get; set; } = 10;
        public bool Resizable { get; set; } = true;

        public event EventHandler SizeChanged;
        public double Width
        {
            get { return ColumnInfo.Width ?? 100; }
            set
            {
                if (!Resizable || value == ColumnInfo.Width || value < MinWidth) return;
                ColumnInfo.Width = value;

                SizeChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        DataTableCell _headerCell;
        public DataTableCell HeaderCell
        {
            get
            {
                if (_headerCell == null)
                {
                    _headerCell = CreateHeaderCell();
                }
                return _headerCell;
            }
        }

        public DataTableColumn()
        {
            //Width = 100;

            //var style = new Style();
            //style.Setters.Add(new Setter(BackgroundProperty, Background));
            //style.Setters.Add(new Setter(ForegroundProperty, Foreground));
            //style.Setters.Add(new Setter(PaddingProperty, Padding));
            //style.Setters.Add(new Setter(HorizontalContentAlignmentProperty, HorizontalContentAlignment));

            //CellStyle = style;
        }
        protected virtual DataTableCell CreateHeaderCell()
        {
            return new DataTableHeaderCell(this);
        }
        public virtual DataTableCell CreateDataCell()
        {
            var cell = this.CreateDataCellCore();
            if (ColumnInfo.BackgroundColor != null)
            {
                cell.Background = Background;
            }
            if (ColumnInfo.Color != null)
            {
                cell.Foreground = Foreground;
            }
            return cell;
        }
        protected abstract DataTableCell CreateDataCellCore();
        public virtual object GetDisplayValue(object value)
        {
            return value;
        }
    }
    public class DataTableTextColumn : DataTableColumn
    {
        public string DisplayFormat => ColumnInfo.DisplayFormat;
        public override object GetDisplayValue(object value)
        {
            if (DisplayFormat == null) return value?.ToString();
            return string.Format("{0:" + DisplayFormat + "}", value);
        }
        protected override DataTableCell CreateDataCellCore()
        {
            return new DataTableTextCell(this);
        }

    }
    public class DataTableCheckColumn : DataTableColumn
    {
        protected override DataTableCell CreateDataCellCore()
        {
            return new DataTableCheckCell();
        }
    }
    public class DataTableColumnCollection : LinkedList<DataTableColumn>
    {
        public DataTableColumn Add(string name, string header)
        {
            var col = new DataTableTextColumn
            {
                Name = name,
                Header = header,
            };
            return this.Add(col);
        }
        public DataTableColumn Add(string name)
        {
            return Add(name, name);
        }
        public DataTableColumn Add(DataTableColumn column)
        {
            base.AddLast(column);
            return column;
        }

        public DataTableColumn this[string name]
        {
            get
            {
                foreach (var col in this)
                {
                    if (col.Name == name)
                    {
                        return col;
                    }
                }
                return null;
            }
        }
        public DataTableColumn this[int index]
        {
            get
            {
                if (index >= Count) return null;
                var col = First;
                for (int i = 0; i < index; i++, col = col.Next) { }

                return col.Value;
            }
        }
    }
}
#endregion

#region Filters
namespace System.Windows.Controls
{
}
#endregion
