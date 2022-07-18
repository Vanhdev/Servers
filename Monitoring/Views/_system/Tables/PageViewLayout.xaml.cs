using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Vst.GUI;

namespace System.Windows.Controls
{
    /// <summary>
    /// Interaction logic for PageViewLayout.xaml
    /// </summary>
    public partial class PageViewLayout : UserControl
    {
        public PageViewLayout()
        {
            InitializeComponent();

            int i = 0;
            foreach (var pageSize in PageSizeOption.Header.Split(','))
            {
                var ps = pageSize + " dòng";
                if (i++ == 1)
                {
                    PageSizeOption.Header = ps;
                }
                var info = new MenuInfo(ps, string.Format($"Ctrl+D{i}"));
                var nav = PageSizeOption.Add(info);
                nav.Click += (s, e) => {
                    PageSizeOption.Header = ps;
                    CreatePages(_items);
                };
            }
            FirstPage.Click += (s, e) => CurrentIndex = 0;
            LastPage.Click += (s, e) => CurrentIndex = _pages.Count - 1;
            NextPage.Click += (s, e) => { if (_currentIndex < _pages.Count - 1) CurrentIndex++; };
            PrevPage.Click += (s, e) => { if (_currentIndex > 0) CurrentIndex--; };
        }

        int _currentIndex = -1;
        public int CurrentIndex
        {
            get => _currentIndex;
            set
            {
                if (_currentIndex != value)
                {
                    _currentIndex = value;

                    CurrentPageButton.Text = string.Format($"{_currentIndex + 1} / {_pages.Count}");
                    CurrentPageChanged?.Invoke(this, null);
                }
            }
        }
        public PageContext CurrentPage => _currentIndex >= _pages.Count ? null : _pages[_currentIndex];

        public event EventHandler CurrentPageChanged;

        List<PageContext> _pages;
        public IEnumerable<PageContext> Pages => _pages;

        System.Collections.IEnumerable _items;
        public void CreatePages(System.Collections.IEnumerable items)
        {
            _currentIndex = -1;
            _items = items;
            _pages = new List<PageContext>();

            int pageSize = GetPageSize();
            int count = 0;

            var lst = new PageContext();
            foreach (var item in items)
            {
                count++;
                lst.Add(item);
                if (lst.Count == pageSize)
                {
                    _pages.Add(lst);
                    lst = new PageContext();
                }
            }
            if (lst.Count > 0)
            {
                _pages.Add(lst);
            }
            CurrentIndex = 0;

            RecordCount.Text = string.Format($"{count} bản ghi");
        }
        int GetPageSize()
        {
            var text = PageSizeOption.Header;
            int a = 0;
            foreach (var c in text)
            {
                if (c < '0' || c > '9') { break; }
                a = (a << 1) + (a << 3) + (c & 15);
            }
            return a;
        }
    }
    public class PageContext : List<object>
    {
    }
}
