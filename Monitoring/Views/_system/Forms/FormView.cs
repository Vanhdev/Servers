using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Windows.Controls
{
    public interface IFormView
    {
        object DataContext { get; set; }
        DataContext EditedValue { get; }
    }

    public abstract class TemplateForm : BaseView<EditForm, DataContext>
    {
        public virtual string TemplateName => ControllerName;

        protected override void RenderCore()
        {
            MainContent.LoadTemplate(TemplateName);
            MainContent.BeginEdit(Model, () => {
                MainContent.Request(ControllerName + "/update", new EditingContext { Value = Model });
            });
        }
    }

}
