using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vst.GUI
{
    public class MenuInfo
    {
        public Action Invoke { get; set; }
        public string IconName { get; set; }
        public string HotKey { get; set; }
        public string Text { get; set; }
        public string Url { get; set; }
        public bool BeginGroup { get; set; }
        public MenuInfoList Childs { get; set; } = new MenuInfoList();
        public bool HasChilds => Childs?.Count > 0;

        public MenuInfo() { }
        public MenuInfo(string text, string hotkey, string url)
        {
            HotKey = hotkey;
            Text = text;
            Url = url;
        }
        public MenuInfo(string text, string hotkey, string url, Action invoke)
            : this(text, hotkey, url)
        {
            Invoke = invoke;
        }
        public MenuInfo(string text, string hotkey)
            : this(text, hotkey, null)
        {
        }
    }
    public class MenuInfoList : List<MenuInfo>
    {
        public MenuInfoList Append(MenuInfo info)
        {
            base.Add(info);
            return this;
        }
        public MenuInfoList Append(IEnumerable<MenuInfo> infos)
        {
            base.AddRange(infos);
            return this;
        }
    }
}
