//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Mqtt
//{
//    public class Vst.Context : DataContext
//    {
//        public string Url { get { return GetString("url"); } set => Push("url", value); }
//        public string ClientId { get { return GetString("clientId"); } set => Push("clientId", value); }
//        public object Value { get { return GetValueCore("value", false); } set => Push("value", value); }
//        public string Token { get { return GetString("#token"); } set => Push("#token", value); }

//        public DataContext GetValueContext()
//        {
//            object value = Value;
//            if (value == null)
//            {
//                return new DataContext();
//            }
//            return FromObject(value);
//        }
//    }
//}
