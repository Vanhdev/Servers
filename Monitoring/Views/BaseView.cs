using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vst.GUI;

namespace System.Windows.Controls
{
    static class VstExtension
    {
        public static void SetAction(this FrameworkElement e, Action callback)
        {
            e.Cursor = System.Windows.Input.Cursors.Hand;
            e.Opacity = 0.8;
            e.MouseMove += (s, a) => e.Opacity = 0.9;
            e.MouseLeave += (s, a) => e.Opacity = 0.8;
            e.MouseLeftButtonDown += (s, a) => e.Opacity = 1;
            e.MouseLeftButtonUp += (s, a) => {
                if (e.Opacity == 1)
                {
                    e.Opacity = 0.8;
                    callback?.Invoke();
                }
            };
        }
    }
    public abstract class BaseView<TView, TModel> : IAppView
        where TView : UIElement, new()
    {
        protected MyMenuBar MainMenuBar
        {
            get
            {
                var layout = (MainLayout)Application.Current.MainWindow.Content;
                return layout.CurrentViewMenu;
            }
        }
        public virtual bool HasContent => true;
        List<ISystemKeyActor> _hotKeys = new List<ISystemKeyActor>();
        protected void SetHotKey(ISystemKeyActor item)
        {
            if (item.HotKey != null)
            {
                _hotKeys.Add(item);
                MyKeyboard.Add(item);
            }
        }
        protected void SetHotKey(System.Collections.IEnumerable items)
        {
            foreach (var item in items)
            {
                if (item is ISystemKeyActor)
                {
                    SetHotKey((ISystemKeyActor)item);
                }
            }
        }
        public virtual void Dispose()
        {
            foreach (var s in _hotKeys)
            {
                MyKeyboard.Remove(s.HotKey);
            }
        }

        protected virtual void CreateMenu(MyMenuBar bar, System.Collections.IEnumerable actions)
        {
            bar.Content.Children.Clear();
            foreach (MenuInfo info in actions)
            {
                if (info.HasChilds)
                {
                    var item = new MyDropDownMenu { Header = info.Text };
                    foreach (MenuInfo child in info.Childs)
                    {
                        SetHotKey(item.Add(child));
                    }
                    bar.Content.Children.Add(item);
                }
                else
                {
                    var item = CreateMenuItemCore(info);
                    if (item != null)
                    {
                        bar.Content.Children.Add(item);
                        SetHotKey(item);
                    }
                }
            }
        }
        protected virtual MyUserControl CreateMenuItemCore(MenuInfo info)
        {
            return new MyMenuItem(info);
        }
        protected virtual MenuInfoList GetActions() { return new MenuInfoList(); }
        protected virtual void CreateActions() { }
        public System.Mvc.ViewDataDictionary ViewBag { get; set; }
        protected TView MainContent { get; private set; }
        protected TModel Model { get; private set; }
        protected virtual TView CreateMainContent()
        {
            return new TView();
        }
        public object Content 
        { 
            get
            {
                CreateActions();
                return MainContent;
            }
        }

        public void Render(object model)
        {
            Model = (TModel)model;
            MainContent = CreateMainContent();

            if (MainContent != null)
            {
                RenderCore();
            }
        }
        protected abstract void RenderCore();
        public virtual string MainCaption => null;
        public virtual string ControllerName => (string)ViewBag["controller"];

        public static bool DisplayAlert(string text, string ok, string cancel)
        {
            string caption = "Server Monitoring";
            if (cancel == null && ok == null)
            {
                MyDialog.Alert(text, caption);
                return true;
            }

            return MyDialog.Confirm(text, caption, ok, cancel).Value;

        }
        public static bool DisplayAlert(string text)
        {
            return DisplayAlert(text, null, null);
        }
        public static bool DisplayConfirm(string text)
        {
            return DisplayAlert(text, "Yes", "No");
        }

        protected void Request(string url, params object[] args)
        {
            _WPF_Extensions.MVC.Execute(url, args);
        }
    }

}
