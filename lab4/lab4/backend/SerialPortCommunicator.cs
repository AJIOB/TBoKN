using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;
using System.Text;
using lab4.backend.exceptions;
using lab4.Enums;

namespace lab4.backend
{
    public class SerialPortCommunicator : INotifyPropertyChanged
    {
        private SerialPort _maskedSerialPort;

        private SerialPort SerialPort
        {
            get => _maskedSerialPort;
            set => ChangeProperty(ref value, ref _maskedSerialPort, nameof(SerialPort));
        }

        private const int BitsInByte = 8;
        private const byte JamByte = 0xFF;
        private const byte EscapeByte = 0x01;
        private const byte JamByteSecondInEscape = 0x00;
        private const byte EscapeByteSecondInEscape = 0x02;
        private const int TimeToWaitNextByteInMs = 100;
        
        private readonly List<byte> _receivedBuffer = new List<byte>();

        public delegate void ReceivedEventHandler(object sender, EventArgs e);

        public event PropertyChangedEventHandler PropertyChanged;

        private bool _isOpen;

        public bool IsOpen
        {
            get => _isOpen;
            private set => ChangeProperty(ref value, ref _isOpen, nameof(IsOpen));
        }

        public static string[] Ports => SerialPort.GetPortNames();

        /// <summary>
        /// Call when property was changed
        /// </summary>
        /// <param name="name"></param>
        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        /// <summary>
        /// Call to change property
        /// </summary>
        /// <typeparam name="T">Property type</typeparam>
        /// <param name="value">New property value</param>
        /// <param name="maskedValue">Field with old property value (masked field)</param>
        /// <param name="propertyName">Name of property</param>
        private void ChangeProperty<T>(ref T value, ref T maskedValue, string propertyName)
        {
            bool isChanged;
            if (maskedValue == null)
            {
                isChanged = (value != null);
            }
            else
            {
                isChanged = !maskedValue.Equals(value);
            }
            maskedValue = value;
            if (isChanged)
            {
                OnPropertyChanged(propertyName);
            }
        }

        /// <summary>
        /// Create communicator and initialize all property changes
        /// </summary>
        public SerialPortCommunicator()
        {
            // if serial port was changed => is open was changed
            PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(SerialPort))
                {
                    IsOpen = (SerialPort != null);
                }
            };
        }

        /// <summary>
        /// Sending info to serial port
        /// </summary>
        /// <param name="s">Sending info</param>
        public void Send(string s)
        {
            InternalLogger.Log.Debug($"Sending string: \"{s}\"");
            var dataToSend = GeneratePacket(Encoding.ASCII.GetBytes(s)).ToArray();
            foreach (var b in dataToSend)
            {
                SerialPort.Write(new []{b}, 0, 1);
            }
        }
        
        /// <summary>
        /// Closing current connection
        /// </summary>
        public void Close()
        {
            if (!IsOpen) return;
            SerialPort.Close();
            SerialPort = null;
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
            SerialPort = new SerialPort(portName, (int) baudRate, parity, (int) dataBits, stopBits);
            SerialPort.Open();
            if (receivedEventHandler != null)
                SerialPort.DataReceived +=
                    delegate(object sender, SerialDataReceivedEventArgs args) { receivedEventHandler(sender, args); };
            _receivedBuffer.Clear();
        }

        /// <summary>
        /// Read existiong string
        /// </summary>
        /// <returns>Existing string</returns>
        public string ReadExisting()
        {
            //Reading data from serial port
            while (SerialPort.BytesToRead > 0)
            {
                var received = SerialPort.ReadByte();
                if (received < 0)
                {
                    InternalLogger.Log.Info("End of the stream was read");
                    break;
                }
                _receivedBuffer.Add((byte) received);
            }

            var parsed = ParsePacket(_receivedBuffer, out var index);
            _receivedBuffer.RemoveRange(0, index);
            
            return Encoding.ASCII.GetString(parsed.ToArray());
        }

        /// <summary>
        /// Encode byte array to bit array with adding bit staffing
        /// </summary>
        /// <param name="inputBytes">Bytes to convert with bit staffing</param>
        /// <returns>Converted input value</returns>
        private IEnumerable<byte> Encode(IEnumerable<byte> inputBytes)
        {
            return inputBytes;
        }

        /// <summary>
        /// Generate packet from data
        /// </summary>
        /// <param name="data">Data to add to packet</param>
        /// <returns>Generated packet</returns>
        private IEnumerable<byte> GeneratePacket(IEnumerable<byte> data)
        {
            return data;
        }

        /// <summary>
        /// Parse packet
        /// </summary>
        /// <param name="packet">Packet to parse</param>
        /// <param name="index">Index of first unused bool element in packet</param>
        /// <returns>Data from packet if packet addressed to me, else return null</returns>
        private IEnumerable<byte> ParsePacket(IEnumerable<byte> packet, out int index)
        {
            var enumerable = packet.ToList();
            index = enumerable.Capacity;
            return enumerable;
        }
    }
}