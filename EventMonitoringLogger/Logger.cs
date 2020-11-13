using System;
using System.IO;

namespace EventMonitoringLogger
{
    public class Logger
    {
        private static readonly string filePath = 
            AppDomain.CurrentDomain.BaseDirectory + @"\failure_log.txt";

        public static async void Log(Exception exception)
        {
            using (StreamWriter streamWriter = File.AppendText(filePath))
            {
                await streamWriter.WriteAsync("\r\nLog Entry : ");
                await streamWriter.WriteLineAsync($"{DateTime.Now:dd/MM/yyyy hh:mm:ss}");
                await streamWriter.WriteLineAsync($"Source       :{exception.Source}");
                await streamWriter.WriteLineAsync($"TargetSite   :[{exception.TargetSite}]");
                await streamWriter.WriteLineAsync($"Description  :{exception.Message}");
                streamWriter.Close();
            }
        }

    }
}
