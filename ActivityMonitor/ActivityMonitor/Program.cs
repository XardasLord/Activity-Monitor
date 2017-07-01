using ActivityMonitor.Resources;
using System;
using System.Collections.Generic;

namespace ActivityMonitor
{
    class Program
    {
        static void Main(string[] args)
        {
            var resourcesToMonitor = new List<IMonitorResource>();

            resourcesToMonitor.Add(FileMonitor.GetInstance());

            foreach(var resource in resourcesToMonitor)
            {
                resource.StartMonitor();
            }
            

            Console.ReadKey();
        }
    }
}
