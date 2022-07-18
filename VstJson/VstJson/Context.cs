using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vst
{
    public class Context : DataContext
    {
        #region Request
        public string Url { get { return GetString("#url"); } set => Push("#url", value); }
        public string ClientId { get { return GetString("#cid"); } set => Push("#cid", value); }
        public string Token { get { return GetString("#token"); } set => Push("#token", value); }
        public string Action { get { return GetString("#action"); } set => Push("#action", value); }
        #endregion

        #region Response
        public object Value { get { return GetValueCore("value", false); } set => Push("value", value); }
        public int Code { get { return GetValue<int>("code"); } set => Push("code", value); }
        public string Message { get { return GetString("message"); } set => Push("message", value); }
        #endregion

        #region VALUES
        DataContext _valueContext;
        public DataContext ValueContext
        {
            get
            {
                if (_valueContext == null)
                {
                    object value = Value;
                    _valueContext = (value == null ? new DataContext() : FromObject(value));
                }
                return _valueContext;
            }
        }
        public T GetValue<T>() => (T)Convert.ChangeType(Value, typeof(T));
        #endregion

        public static implicit operator Context(string s)
        {
            return Parse<Context>(s);
        }
        public static implicit operator Context(byte[] bytes)
        {
            return Parse<Context>(bytes.UTF8());
        }
        public static explicit operator byte[](Context context)
        {
            return context.ToString().UTF8();
        }
    }
}
