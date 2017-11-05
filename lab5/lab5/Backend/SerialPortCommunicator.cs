using System.Collections.Concurrent;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using lab5.Enums;

namespace lab5.Backend
{
    public class SerialPortCommunicator : INotifyPropertyChanged
    {
        private const int ReadTimeoutInMs = 100;
        private const int MaxMessageSize = 64;
        private const byte ServerId = 0;

        private SerialPort _maskedSerial;

        private SerialPort Serial
        {
            get => _maskedSerial;
            set => ChangeProperty(ref value, ref _maskedSerial, nameof(Serial));
        }

        //thread-safe queue
        private readonly ConcurrentPriorityQueue<int, SerialPackage> _transmitBuffer = new ConcurrentPriorityQueue<int, SerialPackage>();

        public delegate void ReceivedEventHandler(string message);

        private ReceivedEventHandler _receivedHandler;

        public event PropertyChangedEventHandler PropertyChanged;

        private bool _isOpen;

        public bool IsOpen
        {
            get => _isOpen;
            private set => ChangeProperty(ref value, ref _isOpen, nameof(IsOpen));
        }

        public byte MyId { private get; set; }

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
                if (args.PropertyName == nameof(Serial))
                {
                    IsOpen = (Serial != null);
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

            if (string.IsNullOrEmpty(s)) return;
            foreach (var i in Enumerable.Range(0, (s.Length + MaxMessageSize - 1) / MaxMessageSize))
            {
                int secondPartLength = s.Length - i * MaxMessageSize;
                string subs = s.Substring(i * MaxMessageSize, secondPartLength >= MaxMessageSize ? MaxMessageSize : secondPartLength);
                SerialPackage package = SerialPackage.GeneratePackage(MyId, destinationId, subs);
                _transmitBuffer.Enqueue(package.Priority, package);
            }
        }

        /// <summary>
        /// Closing current connection
        /// </summary>
        public void Close()
        {
            if (!IsOpen) return;
            Serial.Close();
            Serial = null;
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
            Serial = new SerialPort(portName, (int)baudRate, parity, (int)dataBits, stopBits);
            Serial.Open();

            _receivedHandler = receivedEventHandler;
            _transmitBuffer.Clear();
            
            Serial.DataReceived += delegate { ReadExisting(); };

            //check server & write token of require
            if (MyId == ServerId)
            {
                WritePackageToPort(SerialPackage.GenerateToken());
            }
        }

        /// <summary>
        /// Read existing string
        /// </summary>
        /// <returns>Existing string</returns>
        private void ReadExisting()
        {
            //waiting timeout to receive package
            Thread.Sleep(ReadTimeoutInMs);

            if (Serial == null || !Serial.IsOpen) return;

            int bytesToRead = Serial.BytesToRead;
            byte[] bytes = new byte[bytesToRead];
            Serial.Read(bytes, 0, bytesToRead);

            if (SerialPackage.TryParsePackage(bytes, out SerialPackage package))
            {
                WorkWithReceivedPackage(package);
                if (_transmitBuffer.TryDequeue(out var result))
                {
                    WritePackageToPort(result.Value);
                }
            }
        }

        /// <summary>
        /// Working with received package
        /// </summary>
        /// <param name="package">Received package</param>
        private void WorkWithReceivedPackage(SerialPackage package)
        {
            // Work as destination
            if (MyId == package.DestinationAddress && !package.IsToken)
            {
                _receivedHandler?.Invoke(package.Info);
                package.IsDataGetted = true;
                package.IsRecognized = true;
            }

            // Work as source
            if (MyId != package.SourceAddress || package.IsToken)
            {
                _transmitBuffer.Enqueue(package.Priority, package);
            }
            else if (!package.IsRecognized)
            {
                InternalLogger.Log.Info($"ID {package.DestinationAddress} wasn't recognize data: {package.Info}");
            }
            else if (!package.IsDataGetted)
            {
                InternalLogger.Log.Info($"ID {package.DestinationAddress} wasn't get data: {package.Info}");
            }
        }

        /// <summary>
        /// Write package to serial port
        /// </summary>
        /// <param name="package">Package to write</param>
        private void WritePackageToPort(SerialPackage package)
        {
            InternalLogger.Log.Debug(package.IsToken
                ? $"Token was send. Info: {package.Info}"
                : $"Message was sent: {package.Info}");

            byte[] token = package.BytesToWrite();
            Serial.Write(token, 0, token.Length);

            InternalLogger.Log.Debug($"Queue size on res: {_transmitBuffer.Count}");
        }
    }
}