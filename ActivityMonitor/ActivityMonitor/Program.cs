using ActivityMonitor.Classes;
using System;
using System.Collections.Generic;
using System.Threading;

namespace ActivityMonitor
{
    class Program
    {
        static void Main(string[] args)
        {
            var resourcesToMonitor = new List<IMonitorResource>();

            resourcesToMonitor.Add(FileMonitor.GetInstance());
            resourcesToMonitor.Add(ProcessMonitor.GetInstance());

            foreach(var resource in resourcesToMonitor)
            {
                Thread thread = new Thread(new ThreadStart(resource.StartMonitor));
                thread.Start();
            }
            

            Console.ReadKey();
        }
    }
}
