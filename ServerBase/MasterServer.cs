using Mqtt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vst.Server
{
    public abstract class MasterServer : ServerBase
    {
        protected abstract void ProcessResponse(PublishContext context);
        protected override void MainThread(int interval)
        {
            this.ShareMemory.AsyncReading<PublishContext>(interval, lst => {
                foreach (var res in lst)
                {
                    ProcessResponse(res);
                }
            });
        }
    }
}
