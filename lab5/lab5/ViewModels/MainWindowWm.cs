using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using lab5.Backend;

namespace lab5.ViewModels
{
    public class MainWindowWm : INotifyPropertyChanged
    {
        private const string StartStringText = "Connect";
        private const string StopStringText = "Disconnect";

        public SerialPortCommunicator Communicator { get; } = new SerialPortCommunicator();

        public ObservableCollection<string> Ports { get; } = new ObservableCollection<string>();
        public bool IsPortOpen => Communicator.IsOpen;
        public bool IsPortNotOpen => !IsPortOpen;

        /// <summary>
        /// Parsed source ID
        /// </summary>
        private byte _si;
        /// <summary>
        /// Parsed destination ID
        /// </summary>
        private byte _di;

        public string SourceId {
            get => _si.ToString();
            set => _si = byte.Parse(value);
        }

        public string DestinationId
        {
            get => _di.ToString();
            set => _di = byte.Parse(value);
        }

        public string StartStopButtonText => IsPortOpen ? StopStringText : StartStringText;

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
            InternalLogger.Log.Debug("Refreshing");

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
    }
}