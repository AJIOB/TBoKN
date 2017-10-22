using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
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

        private const byte JamByteToShow = (byte) 'X';

        private const int TimeToWaitNextByteInMs = 5;
        private const int TimeToWaitCollisionInMs = 2 * TimeToWaitNextByteInMs;
        private const int TimesToTryToSendValue = 3;
        private const int TimeSlotValueInMs = TimeToWaitCollisionInMs + TimeToWaitNextByteInMs + 1;

        private CommunicatorStates _state = CommunicatorStates.Standart;

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
            var dataToSend = GeneratePacket(Encoding.UTF8.GetBytes(s)).ToArray();
            foreach (var b in dataToSend)
            {
                SendByte(b);
            }
        }

        private void SendByte(byte b)
        {
            var r = new Random();
            for (var i = 0; ;)
            {
                while (!IsChannelFree())
                {
                    Thread.Sleep(1);
                }

                SerialPort.Write(new[] {b}, 0, 1);
                Thread.Sleep(TimeToWaitCollisionInMs);
                if (!IsCollisionDetected())
                {
                    return;
                }
                SerialPort.Write(new[] {JamByte}, 0, 1);
                i++;

                if (i >= TimesToTryToSendValue)
                {
                    throw new Exception($"So much attempts to write. Cannot write byte {b}"); 
                }
                
                Thread.Sleep(r.Next((2 << i) + 1) * TimeSlotValueInMs);
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
            SerialPort.ReadTimeout = TimeToWaitNextByteInMs;
        }

        /// <summary>
        /// Read existing string
        /// </summary>
        /// <returns>Existing string</returns>
        public string ReadExisting()
        {
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

                var receivedByte = (byte) received;

                //Receiver state machine
                switch (_state)
                {
                    case CommunicatorStates.EscapeFound:
                        _state = CommunicatorStates.Standart;
                        switch (receivedByte)
                        {
                            case EscapeByteSecondInEscape:
                                _receivedBuffer.Add(EscapeByte);
                                break;
                            case JamByteSecondInEscape:
                                _receivedBuffer.Add(JamByte);
                                break;
                            default:
                                _state = CommunicatorStates.JamFound;
                                _receivedBuffer.Clear();
                                parsedList.Add(JamByteToShow);
                                break;
                        }
                        break;
                    case CommunicatorStates.JamFound:
                    case CommunicatorStates.Standart:
                        switch (receivedByte)
                        {
                            case EscapeByte:
                                _state = CommunicatorStates.EscapeFound;
                                parsedList.AddRange(_receivedBuffer);
                                _receivedBuffer.Clear();
                                break;
                            case JamByte:
                                _state = CommunicatorStates.JamFound;
                                _receivedBuffer.Clear();
                                parsedList.Add(JamByteToShow);
                                break;
                            default:
                                _state = CommunicatorStates.Standart;
                                parsedList.AddRange(_receivedBuffer);
                                _receivedBuffer.Clear();
                                _receivedBuffer.Add(receivedByte);
                                break;
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            if (_receivedBuffer.Count > 0)
            {
                parsedList.AddRange(_receivedBuffer);
                _receivedBuffer.Clear();
            }

            return Encoding.UTF8.GetString(parsedList.ToArray());
        }

        /// <summary>
        /// Generate packet from data
        /// </summary>
        /// <param name="data">Data to add to packet</param>
        /// <returns>Generated packet</returns>
        private IEnumerable<byte> GeneratePacket(IEnumerable<byte> data)
        {
            var res = new List<byte>();

            foreach (var b in data)
            {
                switch (b)
                {
                    case JamByte:
                        res.AddRange(new[] {EscapeByte, JamByteSecondInEscape});
                        break;
                    case EscapeByte:
                        res.AddRange(new[] {EscapeByte, EscapeByteSecondInEscape});
                        break;
                    default:
                        res.Add(b);
                        break;
                }
            }

            return res;
        }

        /// <summary>
        /// Cheching is channel free
        /// </summary>
        /// <returns>true if channel is free, else false</returns>
        private bool IsChannelFree()
        {
            return (DateTime.Now.TimeOfDay.Seconds % 2 != 0);
        }

        /// <summary>
        /// Check is collision detected
        /// </summary>
        /// <returns>true if collision was detected, else false</returns>
        private bool IsCollisionDetected()
        {
            return (DateTime.Now.TimeOfDay.Seconds % 2 == 0);
        }
    }

    internal enum CommunicatorStates
    {
        Standart,
        EscapeFound,
        JamFound
    }
}