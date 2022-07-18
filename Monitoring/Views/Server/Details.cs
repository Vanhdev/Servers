using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Monitoring.Views.Server
{
    class Details : BaseView<DetailsLayout, Models.ProcessInfo>
    {
        void CheckState()
        {
            var running = Model.IsRunning;
            MainContent.Dispatcher.InvokeAsync(() => {
                MainContent.StartAction.Visibility = !running ? Visibility.Visible : Visibility.Collapsed;
                MainContent.ShowAction.Visibility = running ? Visibility.Visible : Visibility.Collapsed;
                MainContent.StopAction.Visibility = running ? Visibility.Visible : Visibility.Collapsed;
            });
        }
        public override string MainCaption => "Server information";
        protected override void RenderCore()
        {
            MainContent.DataContext = Model;

            MainContent.ShowAction.Click += (s, e) => { 
                MainContent.Request("Server/Show", Model);
                DisplayAlert("Developing ...");
            };
            MainContent.StartAction.Click += (s, e) => { MainContent.Request("Server/Start", Model); };
            MainContent.StopAction.Click += (s, e) => { 
                if (DisplayConfirm("Stop the " + Model.Name + " server?") == true)
                {
                    MainContent.Request("Server/Stop", Model);
                }
            };
            MainContent.OpenLocationAction.Click += (s, e) => {
                string filePath = Model.FullPath;
                if (!System.IO.File.Exists(filePath))
                {
                    DisplayAlert("File not found.");
                    return;
                }

                string argument = "/select, \"" + filePath + "\"";
                System.Diagnostics.Process.Start("explorer.exe", argument);
            };

            CheckState();
            Model.AliveChanged += (s, e) => CheckState();
        }
    }
}
