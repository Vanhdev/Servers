using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mqtt
{
    public class PublishContext : Vst.Context
    {
        public bool Retain { get { return GetValue<bool>("#retain"); } set => Push("#retain", value); }
        public byte QoS { get { return GetValue<byte>("#qos"); } set => Push("#qos", value); }
        public string Topic { get { return GetString("#topic"); } set => Push("#topic", value); }

        public PublishContext() { }
        public PublishContext(string topic, object value)
        {
            this.Topic = topic;
            this.Value = value;
        }
        public PublishContext(string topic, object value, byte qos)
        {
            this.Topic = topic;
            this.Value = value;
            this.QoS = qos;
        }
        public PublishContext(string topic, object value, byte qos, bool retain)
        {
            this.Topic = topic;
            this.Value = value;
            this.QoS = qos;
            this.Retain = retain;
        }

        public byte[] Payload
        {
            get
            {
                object v = Value;

                if (v is byte[]) { return (byte[])v; }
                
                if (v == null) { return null; }

                if (v is string) { return ((string)v).UTF8(); }
                if (v is JObject || v is JArray) { return v.ToString().UTF8(); }

                try
                {
                    if (Value is Array || Value is System.Collections.IList)
                    {
                        return JArray.FromObject(Value).ToString().UTF8();
                    }
                }
                catch
                {

                }
                return JObject.FromObject(Value).ToString().UTF8();
            }
        }
    }
}
