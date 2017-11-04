using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using lab5.Enums;

namespace lab5.Backend
{
    public class SerialPortCommunicator : INotifyPropertyChanged
    {
        private SerialPort _maskedSerialPort;

        private SerialPort SerialPort
        {
            get => _maskedSerialPort;
            set => ChangeProperty(ref value, ref _maskedSerialPort, nameof(SerialPort));
        }

        private readonly List<byte> _receivedBuffer = new List<byte>();

        //thread-safe queue
        private readonly ConcurrentQueue<SerialPackage> _transceiveBuffer = new ConcurrentQueue<SerialPackage>();

        public delegate void ReceivedEventHandler(string message);

        private ReceivedEventHandler _receivedHandler;

        public event PropertyChangedEventHandler PropertyChanged;

        private bool _isOpen;

        public bool IsOpen
        {
            get => _isOpen;
            private set => ChangeProperty(ref value, ref _isOpen, nameof(IsOpen));
        }

        public byte MyId { private get; set; } = 0;

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
        /// <param name="destinationId">Target ID to send</param>
        /// <param name="s">Sending info</param>
        public void Send(byte destinationId, string s)
        {
            InternalLogger.Log.Debug($"Targer ID: \"{destinationId}\"");
            InternalLogger.Log.Debug($"Sending string: \"{s}\"");
            //TODO
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

            _receivedHandler = receivedEventHandler;
            _receivedBuffer.Clear();

            //Clean transcieve buffer
            while (!_transceiveBuffer.IsEmpty)
            {
                _transceiveBuffer.TryDequeue(out SerialPackage _);
            }
            
            SerialPort.DataReceived += delegate { ReadExisting(); };

            //TODO: check server & write it logic
        }

        /// <summary>
        /// Read existing string
        /// </summary>
        /// <returns>Existing string</returns>
        private void ReadExisting()
        {
            //TODO
            var parsedList = new List<byte>();
            //Reading data from serial port
            while (true)
            {
                int received;
                try
                {
                    received = SerialPort.ReadByte();
                }
                catch (TimeoutException)
                {
                    //not receiving more symbols, exiting
                    break;
                }

                if (received < 0)
                {
                    InternalLogger.Log.Info("End of the stream was read");
                    break;
                }

                var receivedByte = (byte)received;
            }

            if (_receivedBuffer.Count > 0)
            {
                parsedList.AddRange(_receivedBuffer);
                _receivedBuffer.Clear();
            }
        }
    }
}