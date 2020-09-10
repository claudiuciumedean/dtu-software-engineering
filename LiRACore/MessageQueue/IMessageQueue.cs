using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LiRACore.MessageQueue
{
    public interface IMessageQueue
    {
         void Send();
         void Receive();

        void Send(string QueueName, string Message);
        void Receive(string QueueName,  Func<string,Task> ConsumeFunc);
    }
}
