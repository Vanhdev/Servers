using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Mvc;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Vst.GUI;

namespace Monitoring
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            this.InitializeComponent();
        }
        public static MainLayout MainLayout { get; private set; }
        protected override void OnStartup(StartupEventArgs ev)
        {
            var layout = new MainLayout();
            layout.btnBack.Click += (s, e) =>
            {
                //_WPF_Extensions.MvcEngine.Execute(HistoryContext.Back);
            };

            layout.btnForward.Click += (s, e) =>
            {
                //_WPF_Extensions.MvcEngine.Execute(HistoryContext.Forward);
            };


            object currentView = null;
            _WPF_Extensions.MVC.Register(this, result =>
            {
                var view = result.View;
                if (view.Content is Window)
                {
                    ((Window)view.Content).ShowDialog();
                    return;
                }

                if (((IAppView)view).HasContent)
                {
                    ((IDisposable)currentView)?.Dispose();
                    currentView = view;

                    //HistoryContext.Push(_WPF_Extensions.MvcEngine.RequestContext);
                    layout.UpdateView((IAppView)view);
                }
            });

            GUI.Load(Environment.CurrentDirectory + "/template/");
            var window = new Window
            {
                Title = "Server Monitoring",
                Visibility = Visibility.Visible,
                WindowState = WindowState.Maximized,
                Content = layout
            };

            var mainMenu = GUI.ReadTemplate<MenuInfo>("menu");
            layout.CreateMenu(mainMenu);

            MainLayout = layout;
            var keyboard = new MyKeyboard();

            window.KeyUp += (s, e) =>
            {
                keyboard.OnKey(e);
            };

            MainWindow = window;
            window.Request("home");
        }
        protected override void OnExit(ExitEventArgs e)
        {
            Engine.Exit();
            base.OnExit(e);
        }
    }
}
