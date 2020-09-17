using System;
using TemperatureScanData.Models;

namespace TemperatureScanData
{
    public class TemperatureData
    {
        public static void SaveTemperatureData(ScanResult scanResult)
        {
            using (TemperatureScanningDBContext dBContext = new TemperatureScanningDBContext())
            {
               try
                {
                    dBContext.ScanResults.Add(scanResult);
                    dBContext.SaveChanges();

                    // получаем объекты из бд и выводим на консоль
                    var scanResults = dBContext.ScanResults;
                    Console.WriteLine("Список объектов:");
                    foreach (ScanResult sc in scanResults)
                    {
                        Console.WriteLine("{0}.{1} - {2}", sc.RecordID, sc.FullName, sc.EventTime);
                    }
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.Error.WriteLine(ex.Message);
                    Console.ResetColor();
                }
            }
        }
    }
}
