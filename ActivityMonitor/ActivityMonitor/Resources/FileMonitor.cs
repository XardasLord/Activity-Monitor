using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActivityMonitor.Resources
{
    public class FileMonitor : IMonitorResource
    {
        private static FileMonitor _instance;
        private static readonly object _sync = new object();
        private static FileSystemWatcher _watcher;

        private FileMonitor()
        {
        }

        public static FileMonitor GetInstance()
        {
            if(_instance == null)
            {
                lock (_sync)
                {
                    _instance = new FileMonitor();
                }
            }

            return _instance;
        }

        public void StartMonitor()
        {
            _watcher = new FileSystemWatcher();
            _watcher.Path = Directory.GetDirectoryRoot(Directory.GetCurrentDirectory());

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
            Console.WriteLine($"File {e.ChangeType} {e.FullPath} at {DateTime.UtcNow}");
        }

        private void OnRenamed(object sender, RenamedEventArgs e)
        {
            Console.WriteLine($"File {e.OldFullPath} renamed to {e.FullPath} at {DateTime.UtcNow}");
        }
    }
}
