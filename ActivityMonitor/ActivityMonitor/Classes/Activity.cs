using System;

namespace ActivityMonitor.Classes
{
    [Serializable]
    public class Activity
    {
        public string Name { get; set; }
        public ActivityObject Object { get; set; }
        public ActivityType Type { get; set; }
        public DateTime CreatedAt { get; set; }

        public enum ActivityObject
        {
            Error,
            File,
            Directory,
            Process
        }

        public enum ActivityType
        {
            Error,

            // File, Directory
            Changed,
            Created,
            Deleted, 
            Renamed,

            // Process
            Started,
            Stopped,
        }

        public Activity()
        {
        }

        public Activity(string name, ActivityObject obj, ActivityType type)
        {
            Name = name;
            Object = obj;
            Type = type;
            CreatedAt = DateTime.UtcNow;
        }
    }
}
