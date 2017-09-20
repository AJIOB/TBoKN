using System.IO.Ports;
using TOKS_lab1.Enums;

namespace TOKS_lab1.backend
{
    public class SerialPortCommunicator
    {
        private SerialPort _serialPort;

        public bool IsOpen => _serialPort != null;

        /// <summary>
        /// Sending info to serial port
        /// </summary>
        /// <param name="s">Sending info</param>
        public void Send(string s)
        {
            _serialPort.Write(s);
        }

        /// <summary>
        /// Closing current connection
        /// </summary>
        public void Close()
        {
            if (!IsOpen) return;
            _serialPort.Close();
            _serialPort = null;
        }

        /// <summary>
        /// Open port with settings
        /// </summary>
        /// <param name="portName">Port name</param>
        /// <param name="baudRate">Baudrate</param>
        /// <param name="parity">Parity</param>
        /// <param name="dataBits">Number of data bits</param>
        /// <param name="stopBits">Number of stop bits</param>
        public void Open(string portName, EBaudrate baudRate, Parity parity, EDataBits dataBits, StopBits stopBits)
        {
            if (IsOpen) return;
            _serialPort = new SerialPort(portName, (int) baudRate, parity, (int) dataBits, stopBits);
            _serialPort.Open();
            
        }

        /// <summary>
        /// Read existiong string
        /// </summary>
        /// <returns>Existing string</returns>
        public string ReadExisting()
        {
            return _serialPort.ReadExisting();
        }
    }
}