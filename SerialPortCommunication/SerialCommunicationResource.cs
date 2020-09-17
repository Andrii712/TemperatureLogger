using EventMonitoringLogger;
using System;
using System.IO.Ports;

namespace SerialPortCommunication
{
    sealed class SerialCommunicationResource : SerialPort
    {
        /// <summary>
        /// Initializes a new instance of the SerialPort class (<paramref name="useDefaultVelues"/>).
        /// </summary>
        /// <param name="useDefaultVelues">Indicate to use the default setting for serial port or let a user perform the custom setting.</param>
        public SerialCommunicationResource(bool useDefaultVelues = true)
            : base()
        {
            if (useDefaultVelues)
            {
                SetDefaultPortProperties();
            }
            else
            {
                SetPortName();
                SetPortBaudRate();
                SetPortParity();
                SetPortDataBits();
                SetPortStopBits();
                SetPortHandshake();
            }

            this.ErrorReceived += new SerialErrorReceivedEventHandler(ErrorReceivedHandler);
        }

        /// <summary>
        /// Set up default value for the COM port properties.
        /// </summary>
        public void SetDefaultPortProperties()
        {
            // Assign the COM port number. By default COM13.
            this.PortName = "COM13";

            // Set Baud rate = 115200
            this.BaudRate = 115200;

            // No parity
            this.Parity = Parity.None;

            // Number of data bits = 8
            this.DataBits = 8;

            // One stop bit
            this.StopBits = StopBits.One;
        }

        // Display Port values and prompt user to enter a port.
        public void SetPortName()
        {
            string newPortName;

            Console.WriteLine("Available Ports:");
            foreach (string s in GetPortNames())
            {
                Console.WriteLine("   {0}", s);
            }

            Console.Write("Enter COM port value (Default: {0}): ", PortName);
            newPortName = Console.ReadLine();

            if (newPortName == "" || !(newPortName.ToLower()).StartsWith("com"))
            {
                PortName = newPortName;
            }
        }

        // Display BaudRate values and prompt user to enter a value.
        public void SetPortBaudRate()
        {
            string newBaudRate;

            Console.Write("Baud Rate(default:{0}): ", BaudRate);
            newBaudRate = Console.ReadLine();

            if (!String.IsNullOrEmpty(newBaudRate))
                BaudRate = int.Parse(newBaudRate);
        }

        // Display PortParity values and prompt user to enter a value.
        public void SetPortParity()
        {
            string newParity;

            Console.WriteLine("Available Parity options:");
            foreach (string s in Enum.GetNames(typeof(Parity)))
            {
                Console.WriteLine("   {0}", s);
            }

            Console.Write("Enter Parity value (Default: {0}):", Parity.ToString(), true);
            newParity = Console.ReadLine();

            if (!String.IsNullOrEmpty(newParity))
            {
                Parity = (System.IO.Ports.Parity)Enum.Parse(typeof(Parity), newParity, true);
            }
        }

        // Display DataBits values and prompt user to enter a value.
        public void SetPortDataBits()
        {
            string newDataBits;

            Console.Write("Enter DataBits value (Default: {0}): ", DataBits);
            newDataBits = Console.ReadLine();

            if (!String.IsNullOrEmpty(newDataBits))
            {
                DataBits = int.Parse(newDataBits.ToUpperInvariant());
            }
        }

        // Display StopBits values and prompt user to enter a value.
        public void SetPortStopBits()
        {
            string newStopBits;

            Console.WriteLine("Available StopBits options:");
            foreach (string s in Enum.GetNames(typeof(System.IO.Ports.StopBits)))
            {
                Console.WriteLine("   {0}", s);
            }

            Console.Write("Enter StopBits value (None is not supported and \n" +
             "raises an ArgumentOutOfRangeException. \n (Default: {0}):", StopBits.ToString());
            newStopBits = Console.ReadLine();

            if (newStopBits == "")
            {
                StopBits = (System.IO.Ports.StopBits)Enum.Parse(typeof(StopBits), newStopBits, true);
            }
        }

        public void SetPortHandshake()
        {
            string newHandshake;

            Console.WriteLine("Available Handshake options:");
            foreach (string s in Enum.GetNames(typeof(System.IO.Ports.Handshake)))
            {
                Console.WriteLine("   {0}", s);
            }

            Console.Write("Enter Handshake value (Default: {0}):", Handshake.ToString());
            newHandshake = Console.ReadLine();

            if (!String.IsNullOrEmpty(newHandshake))
            {
                Handshake = (System.IO.Ports.Handshake)Enum.Parse(typeof(System.IO.Ports.Handshake), newHandshake, true);
            }
        }

        private static void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            System.IO.Ports.SerialPort sp = (System.IO.Ports.SerialPort)sender;
            string indata = sp.ReadExisting();
            Console.WriteLine($"Serial Port Data Received: {indata}");
        }

        private static void ErrorReceivedHandler(object sender, SerialErrorReceivedEventArgs e)
        {
            switch (e.EventType)
            {
                case SerialError.Frame:
                    //Console.Error.WriteLine("SerialError: Framing error");
                    Logger.Log(new Exception("SerialError: Framing error"));
                    break;
                case SerialError.Overrun:
                    //Console.Error.WriteLine("A character-buffer overrun. The nex character is lost.");
                    Logger.Log(new Exception("SerialError: A character-buffer overrun. The nex character is lost."));
                    break;
                case SerialError.RXOver:
                    //Console.Error.WriteLine("An input buffer overflow.");
                    //Console.Error.WriteLine("There is either no room in the input buffer, or a character was received after the EOF character.");
                    Logger.Log(new Exception("SerialError: An input buffer overflow.\nThere is either no room in the input buffer, or a character was received after the EOF character.")); 
                    break;
                case SerialError.RXParity:
                    //Console.Error.WriteLine("The hardware detected a parity error");
                    Logger.Log(new Exception("SerialError: The hardware detected a parity error"));
                    break;
                case SerialError.TXFull:
                    //Console.Error.WriteLine("Tried to transmit a character, but the output buffer was full.");
                    Logger.Log(new Exception("SerialError: Tried to transmit a character, but the output buffer was full."));
                    break;
            }
        }
    }
}
