using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public class DataContext : Dictionary<string, object>
    {
        #region CLONE
        public DataContext Copy(DataContext src, params string[] names)
        {
            if (names.Length == 0)
            {
                names = src.Keys.ToArray();
            }
            foreach (var name in names)
            {
                if (this.ContainsKey(name) == false)
                {
                    var v = src[name];
                    if (v != null)
                    {
                        base.Add(name, v);
                    }
                }
            }
            return this;
        }
        public DataContext Move(DataContext dst, params string[] names)
        {
            foreach (var name in names)
            {
                var v = this.GetValueCore(name, true);
                dst.Push(name, v);
            }
            return dst;
        }
        public T ChangeType<T>() where T : DataContext, new()
        {
            var dst = new T();
            dst.Copy(this);

            return dst;
        }
        public T ToObject<T>()
        {
            return JObject.FromObject(this).ToObject<T>();
        }
        public static T Parse<T>(string text) where T: DataContext
        {
            return JObject.Parse(text).ToObject<T>();
        }
        public static DataContext Parse(string text)
        {
            return JObject.Parse(text).ToObject<DataContext>();
        }
        public static T FromObject<T>(object src) where T: DataContext
        {
            return JObject.FromObject(src).ToObject<T>();
        }
        public static DataContext FromObject(object src)
        {
            return JObject.FromObject(src).ToObject<DataContext>();
        }
        #endregion

        protected virtual object GetValueCore(string name, bool remove)
        {
            object value;
            if (base.TryGetValue(name, out value))
            {
                if (remove)
                {
                    base.Remove(name);
                }
            }
            return value;
        }
        public void Push(string name, object value)
        {
            if (value == null) return;
            if (base.ContainsKey(name))
            {
                base[name] = value;
            }
            else
            {
                base.Add(name, value);
            }
        }
        public object Pop(string name)
        {
            return GetValueCore(name, true);
        }
        public T Pop<T>(string name)
        {
            return (T)(GetValueCore(name, true) ?? default(T));
        }
        public override string ToString()
        {
            return JObject.FromObject(this).ToString();
        }

        #region GET ITEMS VALUES
        public T GetObject<T>(string name)
        {
            object v;
            if (!TryGetValue(name, out v))
            {
                return default(T);
            }

            var obj = v is string ? JObject.Parse((string)v) : JObject.FromObject(v);
            return obj.ToObject<T>();
        }
        public T GetArray<T>(string name)
        {
            object v;
            if (!TryGetValue(name, out v))
            {
                return default(T);
            }

            if (v.GetType() == typeof(T)) { return (T)v; }

            var obj = v is string ? JArray.Parse((string)v) : JArray.FromObject(v);
            return obj.ToObject<T>();
        }
        public T GetValue<T>(string name)
        {
            object v;
            if (TryGetValue(name, out v))
            {
                return (T)Convert.ChangeType(v, typeof(T));
            }
            return default(T);
        }
        public virtual string GetString(string name) => GetValue<string>(name);
        #endregion

        #region SET ITEMS VALUES
        public void SetObject(string name, object value)
        {
            Push(name, JObject.FromObject(value).ToString());
        }
        public void SetArray(string name, object value)
        {
            Push(name, JArray.FromObject(value).ToString());
        }
        public virtual void SetString(string name, string value) => Push(name, value);
        public void UpdateObject<T>(string name, Func<T, bool> updateCallback)
            where T: DataContext, new()
        {
            var obj = GetObject<T>(name) ?? new T();
            if (updateCallback(obj))
            {
                Push(name, obj);
            }
        }    
        #endregion

        #region ObjectId
        protected virtual string CreateObjectId() { return string.Empty; }
        public string ObjectId
        {
            get
            {
                var id = GetString("_id");
                if (id == null)
                {
                    Add("_id", id = CreateObjectId());
                }
                return id;
            }
            set
            {
                Push("_id", value);
            }
        }
        #endregion
    }

    public enum EditingActions
    {
        Delete = -1, Update, Insert
    }
    public class EditingContext : DataContext
    {
        public EditingActions Action { get; set; }
        public DataContext Value { get; set; }
    }
}
