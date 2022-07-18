using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uPLibrary.Networking.M2Mqtt;

namespace ClientDemo
{
    public class Mqtt
    {
        static MqttClient _mqttClient;

        public MqttClient Client => _mqttClient;
        public string ClientId => _mqttClient.ClientId;
        public Vst.Context Response { get; set; }

        public event EventHandler ConnectionChanged;
        public event EventHandler MessageReceived;

        protected string GetDefaultSubscribeTopic(string prefix)
        {
            return prefix + '/' + _mqttClient.ClientId;
        }
        protected virtual void OnConnectError(Exception e) 
        {
            Console.WriteLine(e.Message);
        }
        public bool IsConnected => _mqttClient != null && _mqttClient.IsConnected;

        const string _cid = "DuyAnhDoHoi";
        const string _topic = "nichirin";
        const string host = "localhost";
        //const string host = "system.aks.vn";
        const int port = 1883;

        public virtual void Connect(string clientId)
        {

            if (_mqttClient != null && _mqttClient.IsConnected) { return; }

            Console.Write("Connect to " + host + "...");
            try
            {
                _mqttClient = new MqttClient(
                    host,
                    port,
                    false,
                    MqttSslProtocols.None,
                    null,
                    null
                );
                _mqttClient.MqttMsgPublishReceived += (s, e) => {
                    var payload = e.Message.ASCII();
                    Response = DataContext.Parse<Vst.Context>(payload);

                    MessageReceived?.Invoke(this, EventArgs.Empty);
                };

                _mqttClient.ConnectionClosed += (s, e) => RaiseConnectionChanged();
                if (clientId == null)
                {
                    clientId = _cid;
                    //clientId = Guid.NewGuid().ToString();
                }
                _mqttClient.Connect(clientId);
                if (_mqttClient.IsConnected)
                {
                    RaiseConnectionChanged();
                    Subscribe(GetDefaultSubscribeTopic("response/default"));
                }
            }
            catch (Exception e)
            {
                OnConnectError(e);
            }
        }
        protected void Disconnect()
        {
            try
            {
                if (_mqttClient != null && _mqttClient.IsConnected)
                {
                    _mqttClient.Disconnect();
                }
            }
            catch
            {
            }
        }
        protected virtual void RaiseConnectionChanged()
        {
            ConnectionChanged?.Invoke(this, EventArgs.Empty);
        }

        public void Publish(string topic, string url)
        {
            this.Publish(topic, url, new { });
        }
        public void Publish(string topic, string url, object value)
        {
            if (_mqttClient != null)
            {
                if (value == null) { value = 0; }
                var context = new Vst.Context
                {
                    Url = url,
                    Value = value,
                };
                Publish(topic ?? _topic, context);
            }
        }
        public virtual void Publish(string topic, Vst.Context context)
        {
            if (_mqttClient != null)
            {
                _mqttClient.Publish(topic ?? _topic, context.ToString().UTF8());
            }
        }
        protected void Subscribe(string topic)
        {
            _mqttClient.Subscribe(new string[] { topic }, new byte[] { 0 });
        }
    }
}
