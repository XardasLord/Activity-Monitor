using ActivityMonitor.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace ActivityMonitor
{
    class Program
    {
        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();
        private static IntPtr ThisConsole = GetConsoleWindow();[DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        private static Int32 showWindow = 0;

        private static NotifyIcon _trayIcon;

        static void Main(string[] args)
        {
            CreateTray();
            ShowWindow(ThisConsole, showWindow);

            var currectDisk = Directory.GetDirectoryRoot(Directory.GetCurrentDirectory());
            var windowsDisk = Directory.GetDirectoryRoot(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));

            var resourcesToMonitor = new List<IMonitorResource>();

            if (!currectDisk.Equals(windowsDisk))
                resourcesToMonitor.Add(new FileMonitor(windowsDisk));
            resourcesToMonitor.Add(new FileMonitor(currectDisk));
            resourcesToMonitor.Add(ProcessMonitor.GetInstance());

            foreach(var resource in resourcesToMonitor)
            {
                Thread thread = new Thread(new ThreadStart(resource.StartMonitoring));
                thread.Start();
            }

            Application.Run();
        }

        private static void CreateTray()
        {
            _trayIcon = new NotifyIcon();
            _trayIcon.Icon = Properties.Resources.Icon;
            _trayIcon.Text = "Activity Monitor";
            _trayIcon.Visible = true;

            _trayIcon.ContextMenuStrip = new ContextMenuStrip();
            _trayIcon.ContextMenuStrip.Items.AddRange(new ToolStripItem[] { new ToolStripMenuItem() });
            _trayIcon.ContextMenuStrip.Items[0].Text = "Exit";
            _trayIcon.ContextMenuStrip.Items[0].Click += new EventHandler(ExitApplication);

            _trayIcon.MouseClick += TrayIcon_MouseClick;
        }

        private static void TrayIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
            {
                showWindow = ++showWindow % 2;
                ShowWindow(ThisConsole, showWindow);
            }
        }

        private static void ExitApplication(object sender, EventArgs e)
        {
            _trayIcon.Visible = false;
            Application.Exit();
            Environment.Exit(1);
        }
    }
}
