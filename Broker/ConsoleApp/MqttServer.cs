using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Server;
using Newtonsoft.Json.Linq;
using Vst.Server;

namespace ConsoleApp
{
    class MqttServer
    {
        IMqttServer _broker;
        bool _publishing;
        public event Action<string, bool> ClientConnectionChanged;
        public event Action<object> MessageReceived;

        bool ProcessMessage(ref MqttApplicationMessageInterceptorContext context)
        {
            if (_publishing)
            {
                _publishing = false;
                return true;
            }

            try
            {
                Screen.Warning("[PUBLISH] " + context.ApplicationMessage.Topic);

                string topic = context.ApplicationMessage.Topic;
                if (topic == null) { return false; }
                if (topic.Contains('/')) { return true; }

                ShareMemory sm = null;
                if (ShareMemory.Open(topic, e => sm = e))
                {
                    try
                    {
                        var payload = context.ApplicationMessage.Payload;
                        var clientId = Encoding.ASCII.GetBytes(",\"#cid\":\"" + context.ClientId + "\"}");

                        var message = new byte[payload.Length + clientId.Length - 1];
                        payload.CopyTo(message, 0);
                        clientId.CopyTo(message, payload.Length - 1);
                        sm.WriteBytes(message);
                    }
                    catch
                    {
                    }
                    return false;
                }
            }
            catch (Exception e)
            {
                Screen.Error(e.Message);
            }
            return true;
        }
        public async void Start()
        {
            var optionsBuilder = new MqttServerOptionsBuilder()
                 .WithConnectionBacklog(100)
                 .WithDefaultEndpointPort(1883)
                 .WithApplicationMessageInterceptor(context => {
                     bool accept = ProcessMessage(ref context);
                     MessageReceived?.Invoke(context);

                     context.AcceptPublish = accept;
                 })
                 .Build();

            _broker = new MqttFactory().CreateMqttServer();

            _broker.ClientConnected += _broker_ClientConnected;
            _broker.ClientDisconnected += _broker_ClientDisconnected;
            _broker.ClientSubscribedTopic += _broker_ClientSubscribedTopic;
            _broker.ClientUnsubscribedTopic += _broker_ClientUnsubscribedTopic;

            await _broker.StartAsync(optionsBuilder);

        }

        private void _broker_ClientUnsubscribedTopic(object sender, MqttClientUnsubscribedTopicEventArgs e)
        {
            Screen.Info("[-UNSUBSCRIBE] " + e.TopicFilter);
        }
        private void _broker_ClientSubscribedTopic(object sender, MqttClientSubscribedTopicEventArgs e)
        {
            Screen.Info("[+SUBSCRIBE] " + e.TopicFilter.Topic);
        }
        private void _broker_ClientDisconnected(object sender, MqttClientDisconnectedEventArgs e)
        {
            Screen.Message("[" + DateTime.Now.ToString("HH:mm:ss") + "] " + e.ClientId);
            ClientConnectionChanged?.Invoke(e.ClientId, false);
        }
        private void _broker_ClientConnected(object sender, MqttClientConnectedEventArgs e)
        {
            Screen.Success("[" + DateTime.Now.ToString("HH:mm:ss") + "] " + e.ClientId);
            ClientConnectionChanged?.Invoke(e.ClientId, true);
        }
        public MqttServer()
        {
        }
        public async void Publish(Mqtt.PublishContext context)
        {
            try
            {
                if (context.Value != null)
                {
                    _publishing = true;
                    var am = new MqttApplicationMessage
                    {
                        Topic = context.Topic,
                        Payload = context.Payload,
                        QualityOfServiceLevel = (MQTTnet.Protocol.MqttQualityOfServiceLevel)context.QoS,
                        Retain = context.Retain,
                    };
                
                    Screen.Message("[" + context.Topic + "] " + context.Value.ToString());

                    await _broker.PublishAsync(am);
                }
            }
            catch (Exception e)
            {
                Screen.Error(e.Message);
            }
        }
    }
}
