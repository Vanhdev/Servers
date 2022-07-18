using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Vst.GUI;
using System.Text;
using System.Threading.Tasks;

namespace System.Windows.Controls
{
    public class FormContent : WrapPanel, IFormView
    {
        GuiMap<IEditor> _editors = new GuiMap<IEditor>();
        public GuiMap<IEditor> Editors => _editors;

        protected override Size MeasureOverride(Size constraint)
        {
            double w = constraint.Width - 10;
            foreach (var editor in _editors.Values)
            {
                var e = (UserControl)editor;
                e.Width = w * editor.Size / 100;
            }
            return base.MeasureOverride(constraint);
        }

        public DataContext EditedValue
        {
            get
            {
                var context = new DataContext();
                foreach (var p in Editors)
                {
                    object v = p.Value.Value;
                    if (p.Value.Text.EndsWith("(*)") && v == null)
                    {
                        return null;
                    }
                    context.Add(p.Key, v);
                }
                return context;
            }
        }

        public FormContent()
        {
        }

        public UIElement Add(string name, IEditor editor)
        {
            _editors.Add(name, editor);
            
            var e = (UIElement)editor;
            Children.Add(e);

            return e;
        }
        public IEditor CreateInput(string type)
        {
            if (string.IsNullOrEmpty(type)) type = "Text";

            var e = (IEditor)Activator.CreateInstance(Type.GetType("System.Windows.Controls." + type + "Input"));
            return e;
        }
        public void LoadTemplate(FieldCollection fields)
        {
            foreach (var p in fields)
            {
                var editor = CreateInput(p.Value.Input);
                var caption = p.Value.Caption;

                if (p.Value.Required == true) caption += " (*)";

                editor.Text = caption;
                editor.Size = p.Value.InputSize ?? 100;
                editor.Name = p.Key;
                editor.Value = p.Value.Value;

                editor.SetOptions(p.Value.Options);

                this.Add(p.Key, editor);
            }
        }
    }

}
