using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace System.Windows.Controls
{
    public static class _WPF_Extensions
    {
        #region MVC
        static System.Mvc.Engine _mvc = new Mvc.Engine();
        public static Mvc.Engine MVC => _mvc;
        public static void Request(this FrameworkElement e, string url, params object[] args)
        {
            if (url != null)
            {
                
                _mvc.Execute(new Mvc.RequestContext(url, args));
            }
        }
        public static void Request(this MyButtonBase e)
        {
            if (e.Url != null)
            {
                _mvc.Execute(e.Url);
            }
        }
        #endregion


        static bool mouseDown = false;
        public static void ApplyMouseClickAction(this UIElement e, Action callback)
        {

            e.MouseLeftButtonDown += (s, a) => mouseDown = true;
            e.MouseLeave += (s, a) => mouseDown = false;
            e.MouseLeftButtonUp += (s, a) => {
                if (!mouseDown) return;
                if (callback != null)
                {
                    callback?.Invoke();
                }

                var ev = e.GetType().GetEvent("Click");
                if (ev != null)
                {
                    var method = ev.GetRaiseMethod();
                    method?.Invoke(e, new object[] { e, EventArgs.Empty });
                }
                
            };
        }

        #region STYLE
        public static void Merge(this Style dst, Style src)
        {
            if (src == null || dst.IsSealed) return;
            foreach (var s in src.Setters) dst.Setters.Add(s);
            
            foreach (var t in src.Triggers)
            {
                dst.Triggers.Add(t);
            }
        }
        static Style LoadStyleCore(Type type)
        {
            var src = (Style)Application.Current.TryFindResource(type);
            if (src == null)
            {
                src = (Style)Application.Current.TryFindResource(type.Name);

                var parent = LoadStyleCore(type.BaseType);
                var style = new Style();
                style.Merge(parent);
                style.Merge(src);

                if (src != null)
                {
                    Application.Current.Resources.Add(type, style);
                }

                return style;
            }

            return src;
        }
        public static void ApplyStyleResource(this FrameworkElement e)
        {
            if (e.Style == null)
            {
                e.Style = LoadStyleCore(e.GetType());
            }
        }
        #endregion

        public static void BeginTimer(this FrameworkElement e, double milliseconds, Func<bool> running)
        {
            var clock = new Threading.DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(milliseconds)
            };
            clock.Tick += (s, a) => {

                if (running() == false)
                {
                    clock.Stop();
                }

            };
            clock.Start();
        }
    }
    public class MyUserControl<T> : UserControl, ISystemKeyActor
        where T: Panel, new()
    {
        new public T Content
        {
            get => (T)base.Content;
            set => base.Content = value;
        }
        public MyUserControl()
        {
            base.Content = new T();

            this.ApplyStyleResource();
            this.ApplyMouseClickAction(() => Activate());
        }
        public string HotKey { get; set; }
        public virtual void Activate() { }
        public bool IsHidden
        {
            get => !IsVisible;
            set => Visibility = value ? Visibility.Hidden : Visibility.Visible;
        }
    }
    public class MyUserControl : MyUserControl<Grid> { }
}
