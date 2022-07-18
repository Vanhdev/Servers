using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Monitoring.Views._Tool
{
    class Index : BaseView<Grid, string>
    {
        public override string MainCaption => "Tool";
        protected override void RenderCore()
        {
            var textBox = new TextInput {
                Text = "Model Name",
            };
            var re = new TextBox { };

            MainContent.Margin = new System.Windows.Thickness(50);
            MainContent.Add(textBox);
            MainContent.Add(re, 1, 0);

            MainContent.RowDefinitions[0].Height = GridLength.Auto;
            textBox.Editor.KeyUp += (s, e) =>
            {
                if (e.Key == System.Windows.Input.Key.Enter)
                {
                    var name = textBox.Value?.ToString();
                    if (string.IsNullOrWhiteSpace(name)) { return; }

                    var varName = string.Format("_{0}{1}", char.ToLower(name[0]), name.Substring(1));
                    re.Text = string.Format(Model, name, varName);

                    Clipboard.SetText(re.Text);
                }
            };
        }
    }
}
