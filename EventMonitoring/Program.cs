using EventMonitoring.Data;
using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace EventMonitoring
{
    class Program
    {
        static void Main()
        {
            ResourceManager stringManager = 
                new ResourceManager("en-US", Assembly.GetExecutingAssembly());

            using (MonitorChange monitor = new MonitorChange())
            {
                monitor.StartMonitorTableChange();

                Console.WindowHeight = 16;
                Console.WindowWidth = 85;
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(stringManager.GetString(
                    "Monitor is running", CultureInfo.CurrentUICulture));
                Console.WriteLine(stringManager.GetString(
                    "Press any key to stop...", CultureInfo.CurrentUICulture));
                Console.ReadKey();

                monitor.StopMonitorTableChange();
            }
        }
    }
}
