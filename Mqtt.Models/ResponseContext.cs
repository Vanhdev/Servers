//using Newtonsoft.Json.Linq;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Mqtt
//{
//    public class Vst.Context : DataContext
//    {
//        public const string DefaultTopic = "response/default/";
//        public int Code { get { return GetValue<int>("Code"); } set => Push("Code", value); }
//        public string Message { get { return GetString("Message"); } set => Push("Message", value); }
//        public object Value { get { return GetValueCore("Value", false); } set => Push("Value", value); }
//        public string Action { get { return GetString("Action"); } set => Push("Action", value); }
//        DataContext _valueContext;
//        public DataContext ValueContext
//        {
//            get
//            {
//                if (_valueContext == null)
//                {
//                    try
//                    {
//                        _valueContext = DataContext.FromObject(Value);
//                    }
//                    catch
//                    {
//                        _valueContext = new DataContext();
//                    }
//                }
//                return _valueContext;
//            }
//        }
//    }
//}
