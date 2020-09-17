using EventMonitoring.Models;
using SerialPortCommunication;
using System;
using System.Threading.Tasks;

namespace EventMonitoring.Data
{
    public static class TemperatureWriter
    {
        public static async Task WriteTemperatureLogAsync(decimal eventNumber, ModbusPortCommunication portCommunication)
        {
            (int relayOut, double lastScanTemp) = await Task.Run(() =>
                portCommunication?.ReadLastInputData() ?? (0, 0.0)).ConfigureAwait(false);

            ScanResult scanResult =
                await ADOSqlClient.ReadFullUserDataAsync(eventNumber).ConfigureAwait(false);

            scanResult.RelayOut = relayOut;
            scanResult.LastScanTemp = lastScanTemp;

            await ADOSqlClient.SaveTemperatureDataAsync(scanResult).ConfigureAwait(false);

#if DEBUG
            Console.WriteLine(scanResult);
#endif
        }
    }
}
