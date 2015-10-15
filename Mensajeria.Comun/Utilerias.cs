using System.Messaging;

namespace Mensajeria.Comun
{
    public class Utilerias
    {
        public static MessageQueue[] ObtenQueues()
        {
            return ObtenQueues("localhost");
        }

        public static MessageQueue[] ObtenQueues(string hostName)
        {
            return MessageQueue.GetPrivateQueuesByMachine(hostName);
        }
    }
}