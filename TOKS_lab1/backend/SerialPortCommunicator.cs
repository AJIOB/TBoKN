using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using TOKS_lab1.Enums;

namespace TOKS_lab1.backend
{
    public class SerialPortCommunicator
    {
        private SerialPort _serialPort;
        private const int BitsInByte = 8;
        private const byte StartStopByte = 0x7E;
        private const byte StartStopReplaceTo = 0x7C;
        private const bool EqualStartStopByteWhenReplacing = false;

        public byte MyId { get; set; } = 0;
        public byte PartnerId { get; set; } = 0;

        public delegate void ReceivedEventHandler(object sender, EventArgs e);

        public bool IsOpen => _serialPort != null;

        /// <summary>
        /// Sending info to serial port
        /// </summary>
        /// <param name="s">Sending info</param>
        public void Send(string s)
        {
            _serialPort.Write(Encoding.ASCII.GetString(GeneratePacket(Encoding.UTF8.GetBytes(s).ToArray()).ToArray()));
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
        /// <param name="receivedEventHandler">Callback to run to when something is received</param>
        public void Open(string portName, EBaudrate baudRate, Parity parity, EDataBits dataBits, StopBits stopBits,
            ReceivedEventHandler receivedEventHandler)
        {
            if (IsOpen) return;
            _serialPort = new SerialPort(portName, (int) baudRate, parity, (int) dataBits, stopBits);
            _serialPort.Open();
            if (receivedEventHandler != null)
                _serialPort.DataReceived +=
                    delegate(object sender, SerialDataReceivedEventArgs args) { receivedEventHandler(sender, args); };
        }

        /// <summary>
        /// Read existiong string
        /// </summary>
        /// <returns>Existing string</returns>
        public string ReadExisting()
        {
            var data = ParsePacket(Encoding.ASCII.GetBytes(_serialPort.ReadExisting()));
            return data != null ? Encoding.UTF8.GetString(data.ToArray()) : "";
        }

        /// <summary>
        /// Encode byte array to bit array with adding bit staffing
        /// </summary>
        /// <param name="inputBytes">Bytes to convert with bit staffing</param>
        /// <returns>Converted input value</returns>
        private IEnumerable<bool> Encode(IEnumerable<byte> inputBytes)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Decode bit array to byte array with deletinig bit staffing
        /// </summary>
        /// <param name="inputBytes">Bits to decode with bit staffing</param>
        /// <returns>Decoded input value</returns>
        private IEnumerable<byte> Decode(IEnumerable<bool> inputBytes)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Check is packet to me
        /// </summary>
        /// <param name="packet">Packet to check</param>
        /// <returns>True if packet addressed to me, else false</returns>
        private bool IsPacketToMe(IEnumerable<byte> packet)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Wrap address metadata to data
        /// </summary>
        /// <param name="data">Data to wrap</param>
        /// <returns>Wrapped string</returns>
        private IEnumerable<byte> WrapAddressMetadata(IEnumerable<byte> data)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Generate packet from data
        /// </summary>
        /// <param name="data">Data to add to packet</param>
        /// <returns>Generated packet</returns>
        private IEnumerable<byte> GeneratePacket(IEnumerable<byte> data)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Parse packet
        /// </summary>
        /// <param name="packet">Packet to parse</param>
        /// <returns>Data from packet if packet addressed to me, else return null</returns>
        private IEnumerable<byte> ParsePacket(IEnumerable<byte> packet)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Converts byte array to bool (bit) array 
        /// </summary>
        /// <param name="data">Data to convert</param>
        /// <returns>Bit array</returns>
        private IEnumerable<bool> BytesToBools(IEnumerable<byte> data)
        {
            var bitArray = new BitArray(data.ToArray());
            var bits = new bool[bitArray.Length];
            bitArray.CopyTo(bits, 0);
            return bits;
        }

        /// <summary>
        /// Converts bool (bit) array to byte array 
        /// </summary>
        /// <param name="data">Data to convert</param>
        /// <returns>Byte array</returns>
        private IEnumerable<byte> BoolsToBytes(IEnumerable<bool> data)
        {
            var bitArray = new BitArray(data.ToArray());
            var bytes = new byte[(bitArray.Length + BitsInByte - 1) / BitsInByte];
            bitArray.CopyTo(bytes, 0);
            return bytes;
        }
    }
}