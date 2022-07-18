using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BsonData
{
    public class BsonDataMap<T> : Dictionary<string, T>
    {
        new public T this[string key]
        {
            get
            {
                key = key.ToLower();
                T value;
                TryGetValue(key, out value);

                return value;
            }
            set
            {
                key = key.ToLower();
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
        new public void Add(string key, T value)
        {
            base.Add(key.ToLower(), value);
        }
        new bool Remove(string key)
        {
            return base.Remove(key.ToLower());
        }
    }
}
