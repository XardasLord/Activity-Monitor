using System.Messaging;

namespace ActivityMonitor.Classes
{
    public interface IQueueService
    {
        void AddMessage(string message);
    }
}
