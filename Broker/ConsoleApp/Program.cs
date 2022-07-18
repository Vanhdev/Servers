using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Vst.Server;
using Mqtt;

namespace ConsoleApp
{
    class Program : MasterServer
    {
        MqttServer _broker;
        static ShareMemory _monitoring;

        protected void Publish(PublishContext context)
        {
            if (_monitoring.Enabled)
            {
                _monitoring.WriteObject(new { type = "publish", context = context });
            }
            _broker.Publish(context);
        }
        protected override void MainThread(int interval)
        {
            _broker = new MqttServer();
            _broker.ClientConnectionChanged += (clientId, isConnected) => {
                this.Publish(new PublishContext {
                    Topic = "connection/" + clientId,
                    Value = new { state = isConnected },
                });
            };
            _broker.MessageReceived += (context) => {
                //if (_monitoring.Enabled)
                //{
                //    _monitoring.WriteObject(new { type = "received", context = context });
                //}
            };

            _broker.Start();
            _monitoring = ShareMemory.Monitoring;


            base.MainThread(interval);
        }
        protected override void ProcessResponse(Mqtt.PublishContext context)
        {
            this.Publish(context);
        }
        protected override bool Wait()
        {
            //Thread.Sleep(100);
            return true;
        }
        static void Main(string[] args)
        {
            new Program().Start();
        }
    }
}
