using System;

namespace ActivityMonitor.Classes
{
    public static class Constants
    {
        public static class Queue
        {
            private static readonly string _myDocumentPath = @Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            public static readonly string FILE_PATH = $@"{_myDocumentPath}\activityLogger.txt";
            public const string QUEUE_PATH = @".\Private$\ActivityMonitor";
            public const double FREQUENCY = 10.0;
        }
    }
}
