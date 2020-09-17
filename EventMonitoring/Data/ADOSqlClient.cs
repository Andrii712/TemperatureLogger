using EventMonitoring.Models;
using EventMonitoringLogger;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace EventMonitoring.Data
{
    static class ADOSqlClient
    {
        internal static string GetConnectionString(string connectionName)
        {
            // To avoid storing the connection string in your code, you can retrieve it from a configuration file.
            //
            // @"Data Source=UACVDB01\SQL2008EXPRESS; Initial Catalog=StopNet4_3; User ID=snadmin; Password=sysadmin;";
            // @"Data Source=UACVDB01\SQL2008EXPRESS; Initial Catalog=TemperatureScanning; User ID=snadmin; Password=sysadmin;";

            return ConfigurationManager.ConnectionStrings[connectionName].ConnectionString;
        }

        internal static async Task<ScanResult> ReadFullUserDataAsync(decimal eventNumber)
        {
            ScanResult scanResult = new ScanResult();

            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString("StopNet4_3DB")))
                {
                    await connection.OpenAsync().ConfigureAwait(false);

                    string selectCmdText =
                        @"SELECT TOP(1) Events.colEventTime AS EventTime
                    ,Events.colAccountID AS AccountID
                    ,(Employees.colSurname + ' ' + Employees.colName + ' ' + Employees.colMiddlename) AS FullName
                    ,Employees.colPost AS Position
                    ,Events.colDirection AS DirectionID
                    ,(CASE
                        WHEN Events.colDirection = 0 THEN 'Entry'
                        ELSE 'Exit'
                    END) AS Direction
                    ,DM01.colID AS DoorModuleID
                    ,DM01.colName AS DoorModule
                    ,Events.colTargetZoneID AS ZoneID
                    ,Zones.colName AS ZoneName
                    FROM [StopNet4_3].[dbo].[tblEvents_55] AS Events, 
                                [StopNet4_3].[dbo].[tblEmployees] AS Employees,
                                [StopNet4_3].[dbo].[tblZones_55] AS Zones,
                                [StopNet4_3].[dbo].[tblControlPointDevices] AS ControlPointDevices, 
                                [StopNet4_3].[dbo].[tblDM01] AS DM01
                    WHERE Events.colEventNumber = @EventNumber
                        AND Events.colHolderID = Employees.colID
                        AND Events.colControlPointID = ControlPointDevices.colControlPointID
                        AND Events.colTargetZoneID = Zones.colID
                        AND ControlPointDevices.colDeviceID = DM01.colID
                    ORDER BY Events.colEventTime DESC;";

                    using (SqlCommand command = new SqlCommand(selectCmdText, connection))
                    {
                        SqlParameter inParamEventNumber = new SqlParameter()
                        {
                            ParameterName = "@EventNumber",
                            SqlDbType = SqlDbType.Decimal,
                            Direction = ParameterDirection.Input,
                            Value = eventNumber
                        };

                        command.Parameters.Add(inParamEventNumber);

                        using (SqlDataReader reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
                        {
                            if (reader.HasRows)
                            {
                                while (await reader.ReadAsync().ConfigureAwait(false))
                                {
                                    scanResult.EventTime = reader.GetDateTime(0);
                                    scanResult.AccountID = reader.GetInt32(1);
                                    scanResult.FullName = reader.GetString(2);
                                    scanResult.Position = reader.GetString(3);
                                    scanResult.DirectionID = reader.GetByte(4);
                                    scanResult.Direction = reader.GetString(5);
                                    scanResult.DoorModuleID = reader.GetInt32(6);
                                    scanResult.DoorModuleName = reader.GetString(7);
                                    scanResult.ZoneID = reader.GetInt32(8);
                                    scanResult.ZoneName = reader.GetString(9);
                                }
                            }
                        }
                    }
                }
            }
            catch (InvalidCastException ex)
            {
                Logger.Log(ex);
            }
            catch (ObjectDisposedException ex)
            {
                Logger.Log(ex);
            }
            catch (InvalidOperationException ex)
            {
                Logger.Log(ex);
            }
            catch (SqlException ex)
            {
                Logger.Log(ex);
            }
            catch (System.IO.IOException ex)
            {
                Logger.Log(ex);
            }

            return scanResult;
        }

        internal static async Task<bool> SaveTemperatureDataAsync(ScanResult scan)
        {
            try
            {
                //SqlTransaction updateTx = null;

                using (SqlConnection connection = new SqlConnection(GetConnectionString("TemperatureScanningDB")))
                {
                    string insertCmdText =
                    "INSERT INTO dbo.ScanResults (EventTime, AccountID, FullName, Position, DirectionID, Direction, DoorModuleID, DoorModuleName, ZoneID, ZoneName, RelayOut, LastScanTemp) " +
                    "VALUES (@EventTime, @AccountID, @FullName, @Position, @DirectionID, @Direction, @DoorModuleID, @DoorModuleName, @ZoneID, @ZoneName, @RelayOut, @LastScanTemp)";

                    await connection.OpenAsync().ConfigureAwait(false);
                    //updateTx = await Task.Run(() => connection.BeginTransaction());

                    using (SqlCommand insertCmd = new SqlCommand(insertCmdText, connection))
                    {
                        //insertCmd.Transaction = updateTx;

                        insertCmd.Parameters.AddWithValue("@EventTime", scan.EventTime);
                        insertCmd.Parameters.AddWithValue("@AccountID", scan.AccountID);
                        insertCmd.Parameters.AddWithValue("@FullName", scan.FullName);
                        insertCmd.Parameters.AddWithValue("@Position", scan.Position);
                        insertCmd.Parameters.AddWithValue("@DirectionID", scan.DirectionID);
                        insertCmd.Parameters.AddWithValue("@Direction", scan.Direction);
                        insertCmd.Parameters.AddWithValue("@DoorModuleID", scan.DoorModuleID);
                        insertCmd.Parameters.AddWithValue("@DoorModuleName", scan.DoorModuleName);
                        insertCmd.Parameters.AddWithValue("@ZoneID", scan.ZoneID);
                        insertCmd.Parameters.AddWithValue("@ZoneName", scan.ZoneName);
                        insertCmd.Parameters.AddWithValue("@RelayOut", scan.RelayOut);
                        insertCmd.Parameters.AddWithValue("@LastScanTemp", scan.LastScanTemp);

                        await insertCmd.ExecuteNonQueryAsync().ConfigureAwait(false);

                        //await Task.Run(() => updateTx.Commit());
                        //Console.WriteLine("Transaction Commit");

                        //await Task.Run(() => updateTx.Rollback());
                        //Console.WriteLine("Transaction Rolled Back");
                        return true;
                    }
                }
            }
            catch (InvalidCastException ex)
            {
                return LogErrorResult(ex);
            }
            catch (ObjectDisposedException ex)
            {
                return LogErrorResult(ex);
            }
            catch (InvalidOperationException ex)
            {
                return LogErrorResult(ex);
            }
            catch (SqlException ex)
            {
                return LogErrorResult(ex);
            }
            catch (System.IO.IOException ex)
            {
                return LogErrorResult(ex);
            }

            bool LogErrorResult(Exception ex)
            {
                Logger.Log(ex);
                return false;
            }
        }

    }
}
