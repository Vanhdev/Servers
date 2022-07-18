using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vst.GUI
{
    public class Field : ColumnInfo
    {
        public bool? Required { get; set; }
        public string Input { get; set; }
        public double? InputSize { get; set; }
        public string Options { get; set; }
    }
    public class GuiMap<T> : Dictionary<string, T>
    {
        new public T this[string key]
        {
            get
            {
                T value;
                base.TryGetValue(key, out value);

                return value;
            }
            set
            {
                if (base.ContainsKey(key))
                {
                    base[key] = value;
                }
                else
                {
                    base.Add(key, value);
                }
            }
        }
    }
    public class FieldCollection<T> : GuiMap<T> where T: ColumnInfo
    {
        public void Copy(FieldCollection source)
        {
            foreach (var p in this)
            {
                var field = source[p.Key];
                if (field != null)
                {
                    p.Value.Copy(field);
                }
            }
        }
    }
    public class FieldCollection : FieldCollection<Field> { }
    public class ColumnCollection : FieldCollection<ColumnInfo> { }
    public class TableInfo
    {
        public string Caption { get; set; }
        public ColumnCollection Columns { get; set; }
    }
    public class TableTemplate : GuiMap<TableInfo>
    {
    }
    public class FormInfo
    {
        public string Caption { get; set; }
        public double Width { get; set; } = 500;
        public FieldCollection Fields { get; set; }
    }
    public class FormTemplate : GuiMap<FormInfo>
    {

    }
    public class GUI
    {
        public static T Read<T>(string filename)
        {
            using (var sw = new System.IO.StreamReader(filename))
            {
                var content = sw.ReadToEnd();
                return JObject.Parse(content).ToObject<T>();
            }
        }
        static public T ReadTemplate<T>(string name)
        {
            return Read<T>(TemplateFolder + name + ".json");
        }

        public static FieldCollection Fields { get; private set; }
        public static FormTemplate Forms { get; private set; }
        public static TableTemplate Tables { get; private set; }
        static public string TemplateFolder { get; private set; }
        static public void Load(string path)
        {
            TemplateFolder = path;
            Fields = Read<FieldCollection>(path + "fields.json");
            Forms = Read<FormTemplate>(path + "forms.json");
            Tables = Read<TableTemplate>(path + "tables.json");

            var defaultField = Fields["#"];
            Fields.Remove("#");

            foreach (var p in Fields)
            {
                var field = p.Value;
                field.Copy(defaultField);
                field.Name = p.Key;
                if (field.Caption == null)
                {
                    field.Caption = p.Key;
                }
            }

            foreach (var form in Forms.Values)
            {
                form.Fields.Copy(Fields);
            }
            foreach (var table in Tables.Values)
            {
                table.Columns.Copy(Fields);
            }
        }
        static public GuiMap<T> Load<T>(string path, string name)
        {
            return Read<GuiMap<T>>(path + "/" + name + ".json");
        }
    }
}
