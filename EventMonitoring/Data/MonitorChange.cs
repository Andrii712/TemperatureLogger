using EventMonitoring.Models; using EventMonitoringLogger;
using SerialPortCommunication;
using System; using System.Collections.Generic;
using System.Threading.Tasks;
using TableDependency.SqlClient; using TableDependency.SqlClient.Base.Enums; using TableDependency.SqlClient.Base.EventArgs;  namespace EventMonitoring.Data {     delegate void PassageThroughTurnstileEventHandler();      public class MonitorChange : IDisposable     {         private bool isDisposed = false;         private readonly SqlTableDependency<tblEvents_55> tblEvents_55Dependency;

        /// <summary>
        /// Represents KeyValue pairs where key is id of control point and value is a particular serial device.
        /// 
        /// Control points ID in the build A:
        ///     9 - ВХІД01,
        ///     23 - ВХІД02,
        ///     25 - ВХІД03,
        ///     27 - ВХІД04.
        /// Control points ID in build B:
        ///     95 - MQB02 — IN01,
        ///     97 - MQB02 — IN02 (Currently used),
        ///     99 - MQB02 — IN03,
        ///     101 - MQB02 — IN04
        /// 
        /// Serial device port name in the build A: COM11.
        /// Serial device port name in the build B: COM13.
        /// </summary>         private Dictionary<int, ModbusPortCommunication> Turnstiles { get; set; }          public MonitorChange()         {             Turnstiles = new Dictionary<int, ModbusPortCommunication>()
            {
                { 9, new ModbusPortCommunication("COM11") },
                { 97, new ModbusPortCommunication("COM13") }
            };              tblEvents_55Dependency = new SqlTableDependency<tblEvents_55>(                 connectionString: ADOSqlClient.GetConnectionString("StopNet4_3DB"),                 schemaName: "dbo",                 notifyOn: DmlTriggerType.Insert);              tblEvents_55Dependency.OnChanged += TableDependency_Changed;             tblEvents_55Dependency.OnError += TableDependency_OnError;         }

        #region TableDependency_Changed handlers          void TableDependency_Changed(object sender, RecordChangedEventArgs<tblEvents_55> e)         {             if (e.ChangeType != ChangeType.None)             {                 var changeEntity = e.Entity;                  if (changeEntity.colDirection == 0  // Direction - entrance                     && changeEntity.colEventCode == 105  // Make an entrance                     && changeEntity.colTargetZoneID == 2  // Internal access system                     && Turnstiles.ContainsKey(changeEntity.colControlPointID))                 {
                   Task.Run(async () =>                          await TemperatureWriter.WriteTemperatureLogAsync(                                 changeEntity.colControlPointID,                                  Turnstiles[changeEntity.colControlPointID])                         .ConfigureAwait(false));                 }             }         }          void TableDependency_OnError(object sender, ErrorEventArgs e)         {             Logger.Log(e.Error);         }

        #endregion

        /// <summary>         /// Start monitoring table's content changing.         /// </summary>         public void StartMonitorTableChange()         {             tblEvents_55Dependency?.Start();         }

        /// <summary>         /// Stop monitoring table's content changing.         /// </summary>         public void StopMonitorTableChange()         {             tblEvents_55Dependency?.Stop();         }

        #region Dispose Pattern Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (isDisposed)
            {
                return;
            }

            if (disposing)
            {
                // Dispose managed state (managed objects).
                tblEvents_55Dependency?.Dispose();
            }

            foreach (KeyValuePair<int, ModbusPortCommunication> entry in Turnstiles)
            {
                entry.Value.Dispose();
            }

            Turnstiles = null;
            isDisposed = true;
        }

        ~MonitorChange() => Dispose(false);

        #endregion
    } } 