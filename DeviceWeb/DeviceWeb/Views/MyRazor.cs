using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace System.Web
{
    public class HtmlField : Vst.GUI.Field
    {
        public string IconName { get; set; }
        public object Value { get; set; }
        public override string ToString()
        {
            var context = new DataContext();
            foreach (var p in this.GetType().GetProperties())
            {
                var v = p.GetValue(this);
                if (v == null) continue;

                var name = char.ToLower(p.Name[0]) + p.Name.Substring(1);
                context.Add(name, v);
            }
            return context.ToString();
        }
    }

    public class HtmlList<T> : List<T>
    {
        public HtmlString ToHtml()
        {
            return new HtmlString(this.ToString());
        }
        public override string ToString()
        {
            var lst = new List<string>();
            foreach (var field in this)
            {
                lst.Add(field.ToString());
            }
            return '[' + string.Join(",", lst) + ']';
        }
    }


    public class HtmlFieldList : HtmlList<HtmlField>
    {

    }



    public class HtmlSection
    {
        public string Text { get; set; } = string.Empty;

        public HtmlFieldList Fields { get; set; } = new HtmlFieldList();

        public override string ToString()
        {
            return ("{ text: '" + Text + "', fields: " + Fields.ToString() + '}');
        }
    }

    public class HtmlSectionList : HtmlList<HtmlSection>
    {

    }
    public class HtmlTable
    {
        public string Caption { get; set; }
        public HtmlFieldList Columns { get; set; }
    }
}