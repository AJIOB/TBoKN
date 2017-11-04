using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO.Ports;
using lab5.Backend;
using lab5.Backend.Exceptions;
using lab5.Enums;
using lab5.Enums.Utilities;

namespace lab5.ViewModels
{
    public class MainWindowWm : INotifyPropertyChanged
    {
        private const string StartStringText = "Connect";
        private const string StopStringText = "Disconnect";

        private SerialPortCommunicator Communicator { get; } = new SerialPortCommunicator();

        #region View bindings

        public ObservableCollection<string> Ports { get; } = new ObservableCollection<string>();
        public bool IsPortOpen => Communicator.IsOpen;
        public bool IsPortNotOpen => !IsPortOpen;

        public string SourceId { get; set; }

        public string DestinationId { get; set; }

        public string PortSelected { get; set; }
        public EnumViewObject BaudrateSelected { get; set; }
        public EnumViewObject DataBitsSelected { get; set; }
        public EnumViewObject StopBitsSelected { get; set; }
        public EnumViewObject ParitySelected { get; set; }

        public string TextToSend { get; set; }

        private string _received = string.Empty;
        public string ReceivedText
        {
            get => _received;
            set => ChangeProperty(ref value, ref _received, nameof(ReceivedText));
        }

        public string StartStopButtonText => IsPortOpen ? StopStringText : StartStringText;

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindowWm()
        {
            Communicator.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(SerialPortCommunicator.IsOpen))
                {
                    OnPropertyChanged(new PropertyChangedEventArgs(nameof(IsPortOpen)));
                    OnPropertyChanged(new PropertyChangedEventArgs(nameof(IsPortNotOpen)));
                    OnPropertyChanged(new PropertyChangedEventArgs(nameof(StartStopButtonText)));
                }
            };
        }

        /// <summary>
        /// Refresh port list
        /// </summary>
        public void PortsRefresh()
        {
            Ports.Clear();
            foreach (var port in SerialPortCommunicator.Ports)
            {
                Ports.Add(port);
            }
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }

        /// <summary>
        /// Call to change property
        /// </summary>
        /// <typeparam name="T">Property type</typeparam>
        /// <param name="value">New property value</param>
        /// <param name="maskedValue">Field with old property value (masked field)</param>
        /// <param name="propertyName">Name of property</param>
        protected void ChangeProperty<T>(ref T value, ref T maskedValue, string propertyName)
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
                OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Close current connection
        /// </summary>
        public void CloseConnection()
        {
            Communicator.Close();
        }

        /// <summary>
        /// Open current connection
        /// </summary>
        public void OpenConnection()
        {
            Communicator.MyId = byte.Parse(SourceId);
            Communicator.Open(PortSelected,
                (EBaudrate)BaudrateSelected.Value,
                (Parity)ParitySelected.Value,
                (EDataBits)DataBitsSelected.Value,
                (StopBits)StopBitsSelected.Value,
                LoadReceivedData);
        }

        /// <summary>
        /// Send text
        /// </summary>
        public void SendText()
        {
            if (!string.IsNullOrEmpty(TextToSend))
            {
                Communicator.Send(byte.Parse(DestinationId), TextToSend);
                TextToSend = string.Empty;
            }
        }

        /// <summary>
        /// Load received data from serial port
        /// </summary>
        private void LoadReceivedData(string message)
        {
            ReceivedText += message;
        }
    }
}