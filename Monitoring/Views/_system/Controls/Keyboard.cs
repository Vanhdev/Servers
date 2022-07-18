using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace System.Windows.Controls
{
    public interface ISystemKeyActor
    {
        string HotKey { get; }
        void Activate();
        bool IsVisible { get; }
    }
    public class MyKeyboard
    {
        static Dictionary<string, ISystemKeyActor> _map = new Dictionary<string, ISystemKeyActor>();

        public static void Add(ISystemKeyActor actor)
        {
            if (string.IsNullOrWhiteSpace(actor.HotKey)) return;

            var key = actor.HotKey.ToUpper();
            if (_map.ContainsKey(key))
            {
                _map[key] = actor;
            }
            else
            {
                _map.Add(key, actor);
            }
        }
        public static void AddRange(System.Collections.IEnumerable items)
        {
            foreach (object item in items)
            {
                var ska = item as ISystemKeyActor;
                if (ska != null)
                {
                    Add(ska);
                }
            }
        }
        public static void Remove(string name)
        {
            name = name.ToUpper();
            if (_map.ContainsKey(name))
            {
                _map.Remove(name);
            }
        }
        public static string CreateHotKey(bool ctrl, bool shift, bool alt, string key)
        {
            var lst = new List<string>();
            if (ctrl) lst.Add("Ctrl");
            if (shift) lst.Add("Shift");
            if (alt) lst.Add("Alt");

            lst.Add(key);
            return string.Join("+", lst);
        }
        public static string CreateHotKey(bool ctrl, bool shift, bool alt, Key key)
        {
            return CreateHotKey(ctrl, shift, alt, key.ToString());
        }

        public void OnKey(KeyEventArgs arg)
        {
            var key = arg.SystemKey;
            if (key == Key.None)
            {
                key = arg.Key;
                if (key == Key.None) return;
            }

            var device = arg.KeyboardDevice;
            var ctrl = device.IsKeyDown(Key.LeftCtrl) | device.IsKeyDown(Key.RightCtrl);
            var alt = device.IsKeyDown(Key.LeftAlt) | device.IsKeyDown(Key.RightAlt);
            var shift = device.IsKeyDown(Key.LeftShift) | device.IsKeyDown(Key.RightShift);

            if (ctrl || alt)
            {
                string name = CreateHotKey(ctrl, shift, alt, key);

                ISystemKeyActor actor;
                if (_map.TryGetValue(name.ToUpper(), out actor))
                {
                    actor.Activate();
                }
            }
        }
    }
}
