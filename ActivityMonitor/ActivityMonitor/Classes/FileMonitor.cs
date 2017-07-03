using System.IO;
using System.Messaging;

namespace ActivityMonitor.Classes
{
    public class FileMonitor : IMonitorResource
    {
        public string Path { get; private set; }
        private static FileSystemWatcher _watcher;

        public FileMonitor(string pathToMonitor)
        {
            Path = pathToMonitor;
        }

        public void StartMonitor()
        {
            _watcher = new FileSystemWatcher();
            _watcher.Path = Path;

            _watcher.NotifyFilter = NotifyFilters.LastWrite |
                                   NotifyFilters.FileName;
            _watcher.Filter = "*.*";
            _watcher.EnableRaisingEvents = true;
            _watcher.IncludeSubdirectories = true;

            _watcher.Changed += new FileSystemEventHandler(OnChanged);
            _watcher.Created += new FileSystemEventHandler(OnChanged);
            _watcher.Deleted += new FileSystemEventHandler(OnChanged);
            _watcher.Renamed += new RenamedEventHandler(OnRenamed);
        }

        public void StopMonitor()
        {
            _watcher.Dispose();
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            var type = Activity.ActivityType.Changed;

            switch(e.ChangeType)
            {
                case WatcherChangeTypes.Changed:
                    type = Activity.ActivityType.Changed;
                    break;
                case WatcherChangeTypes.Created:
                    type = Activity.ActivityType.Created;
                    break;
                case WatcherChangeTypes.Deleted:
                    type = Activity.ActivityType.Deleted;
                    break;
            }

            var activity = new Activity(e.FullPath,
                Activity.ActivityObject.File,
                type);

            var message = new Message(activity);
            AddMessage(message);
        }

        private void OnRenamed(object sender, RenamedEventArgs e)
        {
            var activity = new Activity(e.FullPath,
                Activity.ActivityObject.File,
                Activity.ActivityType.Renamed);

            var message = new Message(activity);
            AddMessage(message);
        }

        private void AddMessage(Message message)
        {
            QueueManager.GetInstance().AddMessage(message);
        }
    }
}
