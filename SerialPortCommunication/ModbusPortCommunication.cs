using EventMonitoringLogger;
using Modbus.Device;
using System;

namespace SerialPortCommunication
{
    public class ModbusPortCommunication : IDisposable
    {
        private bool isDisposed = false;
        private SerialCommunicationResource serialPort;
        private readonly ModbusSerialMaster master;
        private readonly byte slaveAddress;

        /// <summary>
        /// RTU master create connection to serial port
        ///     (<paramref name="serialPort"/>,<paramref name="slaveAddress"/>).
        /// </summary>
        /// <param name="serialPort">A serial port resource.</param>
        /// <param name="slaveAddress">Address of device to read value from.</param>
        public ModbusPortCommunication(string serialPortName = "COM13", byte slaveAddress = 1)
        {
            InitializeSerialPort(serialPortName);

            master = ModbusSerialMaster.CreateRtu(serialPort);

            // Number of times to retry sending message after encountering a failure
            // such as an IOException, TimeoutException, or a corrupt message.
            // Default Value is Retries = 3.
            master.Transport.Retries = 0;

            // Gets or sets the number of milliseconds before a timeout occurs
            // when a read operation does not finis recommended value is ReadTimeout = 300).
            master.Transport.ReadTimeout = 100;

            this.slaveAddress = slaveAddress;
        }

        /// <summary>
        /// Open serial port connection.
        /// </summary>
        /// <param name="portName"></param>
        private void InitializeSerialPort(string portName)
        {
            serialPort = new SerialCommunicationResource()
            {
                PortName = portName
            };

            serialPort.Open();
        }

        /// <summary>
        /// Read contguous block of holding registers.
        /// </summary>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="numOfPoints">Number of holding registers to read.</param>
        public ushort[] ReadHoldingRegisters(ushort startAddress = 0, ushort numOfPoints = 1)
        {
            return master.ReadHoldingRegisters(
                slaveAddress, startAddress, numOfPoints);
        }

        /// <summary>
        /// Read contiguous block of input registers.
        /// </summary>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="numOfPoints">Number of input registers to read.</param>
        /// <returns>Array of register data.</returns>
        public ushort[] ReadInputRegisters(ushort startAddress = 0, ushort numOfPoints = 2)
        {
            return master.ReadInputRegisters(
                slaveAddress, startAddress, numOfPoints);
        }

        /// <summary>
        /// Read contiguous block of input registers.
        /// </summary>
        public (int RelayOut, double LastScanTemp) ReadLastInputData(ushort startAddress = 0, ushort numOfPoints = 2)
        {
            try
            {
                ushort[] inputRegisters = ReadInputRegisters(startAddress, numOfPoints);
                return (RelayOut: Convert.ToInt32(inputRegisters[0]), LastScanTemp: (inputRegisters[1] * 0.1));
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                return (RelayOut: 0, LastScanTemp: 0.0);
            }
        }

        /// <summary>
        /// Read contiguous block of input registers and print the result to the terminal.
        /// </summary>
        public void ReadLastInputDataToConsole(ushort startAddress = 0, ushort numOfPoints = 2)
        {
            try
            {
                ushort[] inputRegisters = ReadInputRegisters(startAddress, numOfPoints);

                if (inputRegisters[1] < 370)
                    Console.ForegroundColor = ConsoleColor.Green;
                else
                    Console.ForegroundColor = ConsoleColor.Red;

                Console.WriteLine($"Scan temperature value: RelayOut - {inputRegisters[0]}, " +
                                $"LastScanTemp - {inputRegisters[1] * 0.1:0.0}");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.Error.WriteLine(ex.Message);
                Console.ResetColor();
            }

            Console.WriteLine(Environment.NewLine);
            Console.WriteLine("Press any key to quit");
            Console.ReadKey();
        }

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
                master?.Dispose();
                serialPort?.Close();
                serialPort?.Dispose();
            }

            isDisposed = true;
        }

        ~ModbusPortCommunication() => Dispose(false);

        #endregion
    }
}
