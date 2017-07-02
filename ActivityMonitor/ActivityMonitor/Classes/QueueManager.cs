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
            InitMessageQueue();

            var startTimeSpan = TimeSpan.Zero;
            var periodTimeSpan = TimeSpan.FromSeconds(Constants.Queue.FREQUENCY);
            var timer = new System.Threading.Timer((e) =>
            {
                lock (_sync)
                {
                    SaveAllMessages();
                    DeleteMessagesFromQueue();
                }
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

        private void InitMessageQueue()
        {
            if (MessageQueue.Exists(Constants.Queue.QUEUE_PATH) == false)
            {
                MessageQueue.Create(Constants.Queue.QUEUE_PATH);
            }

            _msmq = new MessageQueue(Constants.Queue.QUEUE_PATH);
            _msmq.Formatter = new XmlMessageFormatter(new Type[] { typeof(Activity) });
            _msmq.Label = "Activity";
        }

        public void AddMessage(Message message)
        {
            if(_msmq == null)
            {
                InitMessageQueue();
            }

            try
            {
                _msmq.Send(message);
            }
            catch(Exception e)
            {
                _msmq.Send($"Noticed an error: {e.Message}");
            }
            
        }

        private void SaveAllMessages()
        {
            using (StreamWriter sw = new StreamWriter(Constants.Queue.FILE_PATH, true))
            {
                var messages = GetAllMessages();
                if (messages.Length < 1)
                {
                    return;
                }

                sw.WriteLine($"[{DateTime.UtcNow.ToShortDateString()}]");

                foreach (var message in messages)
                {
                    Activity activity = (Activity)message.Body;

                    sw.WriteLine($"{activity.Object} {activity.Name} {activity.Type} at {activity.CreatedAt}.");
                }
            }
        }

        private Message[] GetAllMessages()
        {
            return _msmq.GetAllMessages();
        }

        private void DeleteMessagesFromQueue()
        {
            _msmq.Purge();
        }
    }
}
