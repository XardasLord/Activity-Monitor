using System;
using System.IO;
using System.Messaging;

namespace ActivityMonitor.Classes
{
    public class QueueManager : IQueueService
    {
        private static QueueManager _instance = null;
        private static readonly object _sync = new object();
        private static MessageQueue _msmq;

        private QueueManager()
        {
            if (MessageQueue.Exists(Constants.Queue.QUEUE_PATH) == false)
            {
                MessageQueue.Create(Constants.Queue.QUEUE_PATH);
            }

            _msmq = new MessageQueue(Constants.Queue.QUEUE_PATH);
            _msmq.Label = "Activity";


            var startTimeSpan = TimeSpan.Zero;
            var periodTimeSpan = TimeSpan.FromMinutes(1);
            var timer = new System.Threading.Timer((e) =>
            {
                SaveAllMessages();
                DeleteMessagesFromQueue();
            }, null, startTimeSpan, periodTimeSpan);
        }

        public static QueueManager GetInstance()
        {
            if(_instance == null)
            {
                lock (_sync)
                {
                    _instance = new QueueManager();
                }
            }

            return _instance;
        }

        public void AddMessage(string message)
        {
            try
            {
                _msmq.Send(message);
            }
            catch(Exception e)
            {
                _msmq.Send($"Noticed an error: {e.Message}.");
            }
            
        }

        private void SaveAllMessages()
        {
            using (StreamWriter sw = new StreamWriter(Constants.Queue.FILE_PATH, true))
            {
                var messages = GetAllMessages();
                if(messages.Length < 1)
                {
                    return;
                }

                sw.WriteLine($"[{DateTime.UtcNow.ToShortDateString()}]");

                foreach (var message in messages)
                {
                    sw.WriteLine(message.Body.ToString());
                }
            }
        }

        private Message[] GetAllMessages()
        {
            var messages = _msmq.GetAllMessages();

            return messages;
        }

        private void DeleteMessagesFromQueue()
        {
            _msmq.Purge();
        }
    }
}
