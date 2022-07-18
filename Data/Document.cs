using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BsonData
{
    public class Document : DataContext
    {
        //#region CLONE
        //public Document Copy(Document src, params string[] names)
        //{
        //    if (names.Length == 0)
        //    {
        //        names = src.Keys.ToArray();
        //    }
        //    foreach (var name in names)
        //    {
        //        if (this.ContainsKey(name) == false)
        //        {
        //            var v = src[name];
        //            if (v != null)
        //            {
        //                base.Add(name, v);
        //            }
        //        }
        //    }
        //    return this;
        //}
        //public Document Move(Document dst, params string[] names)
        //{
        //    foreach (var name in names)
        //    {
        //        var v = this.GetValueCore(name, true);
        //        dst.Push(name, v);
        //    }
        //    return dst;
        //}
        //public T ChangeType<T>() where T : Document, new()
        //{
        //    var dst = new T();
        //    dst.Copy(this);

        //    return dst;
        //}
        //public T ToObject<T>()
        //{
        //    return JObject.FromObject(this).ToObject<T>();
        //}
        //#endregion

        //protected virtual object GetValueCore(string name, bool remove)
        //{
        //    object value;
        //    if (base.TryGetValue(name, out value))
        //    {
        //        if (remove)
        //        {
        //            base.Remove(name);
        //        }
        //    }
        //    return value;
        //}
        //public void Push(string name, object value)
        //{
        //    if (value == null) return;
        //    if (base.ContainsKey(name))
        //    {
        //        base[name] = value;
        //    }
        //    else
        //    {
        //        base.Add(name, value);
        //    }
        //}
        //public T Pop<T>(string name)
        //{
        //    return (T)(GetValueCore(name, true) ?? default(T));
        //}

        //public override string ToString()
        //{
        //    return JObject.FromObject(this).ToString();
        //}
        protected override string CreateObjectId()
        {
            return new ObjectId().ToString();
        }

        //#region GET ITEMS VALUES
        //public T GetObject<T>(string name)
        //{
        //    object v;
        //    if (!TryGetValue(name, out v))
        //    {
        //        return default(T);
        //    }

        //    var obj = v is string ? JObject.Parse((string)v) : JObject.FromObject(v);
        //    return obj.ToObject<T>();
        //}
        //public T GetArray<T>(string name)
        //{
        //    object v;
        //    if (!TryGetValue(name, out v))
        //    {
        //        return default(T);
        //    }

        //    var obj = v is string ? JArray.Parse((string)v) : JArray.FromObject(v);
        //    return obj.ToObject<T>();
        //}
        //public T GetValue<T>(string name)
        //{
        //    object v;
        //    if (TryGetValue(name, out v))
        //    {
        //        return (T)Convert.ChangeType(v, typeof(T));
        //    }
        //    return default(T);
        //}
        //public virtual string GetString(string name) => GetValue<string>(name);
        //#endregion

        //#region SET ITEMS VALUES
        //public void SetObject(string name, object value)
        //{
        //    Push(name, JObject.FromObject(value).ToString());
        //}
        //public void SetArray(string name, object value)
        //{
        //    Push(name, JArray.FromObject(value).ToString());
        //}
        //public virtual void SetString(string name, string value) => Push(name, value);
        //#endregion
    }

    public class DocumentMap<T> : Dictionary<string, T>
        where T: Document
    {
        new public T this[string objectId]
        {
            get
            {
                if (string.IsNullOrEmpty(objectId))
                {
                    return default(T);
                }

                T value;
                TryGetValue(objectId, out value);

                return value;
            }
            set
            {
                if (base.ContainsKey(objectId))
                {
                    base[objectId] = value;
                }
                else
                {
                    base.Add(objectId, value);
                }
            }
        }
        public void Add(T doc)
        {
            this[doc.ObjectId] = doc;
        }
        public void AddRange(IEnumerable<T> items)
        {
            foreach (var doc in items)
            {
                this.Add(doc);
            }
        }
        new public DocumentMap<T> Clear()
        {
            base.Clear();
            return this;
        }
    }

    public class DocumentMap : DocumentMap<Document> { }
}
