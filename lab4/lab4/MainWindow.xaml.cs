using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO.Ports;
using System.Windows;
using lab4.backend;
using lab4.backend.exceptions;
using lab4.Enums;
using lab4.Enums.Utilities;

namespace lab4
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private const string ErrorMessageBoxHeader = @"Oops, we have an error";
        private const string StartStringText = "Start";
        private const string StopStringText = "Stop";

        private readonly SerialPortCommunicator _serialPortCommunicator = new SerialPortCommunicator();

        public ObservableCollection<string> Ports { get; } = new ObservableCollection<string>();
        public bool IsPortOpen => _serialPortCommunicator.IsOpen;
        public bool IsPortNotOpen => !IsPortOpen;

        public string StartStopButtonText => IsPortOpen ? StopStringText : StartStringText;

        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindow()
        {
            InitializeComponent();

            _serialPortCommunicator.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(SerialPortCommunicator.IsOpen))
                {
                    OnPropertyChanged(new PropertyChangedEventArgs(nameof(IsPortOpen)));
                    OnPropertyChanged(new PropertyChangedEventArgs(nameof(IsPortNotOpen)));
                    OnPropertyChanged(new PropertyChangedEventArgs(nameof(StartStopButtonText)));
                }
            };
        }

        private void StartStopButtonPressed(object sender, RoutedEventArgs e)
        {
            if (_serialPortCommunicator.IsOpen)
            {
                _serialPortCommunicator.Close();
            }
            else
            {
                try
                {
                    _serialPortCommunicator.Open(CurrentPortComboBox.SelectedItem.ToString(),
                        (EBaudrate) ((EnumViewObject) BaudrateComboBox.SelectedItem).Value,
                        (Parity) ((EnumViewObject) ParityComboBox.SelectedItem).Value,
                        (EDataBits) ((EnumViewObject) DataBitsComboBox.SelectedItem).Value,
                        (StopBits) ((EnumViewObject) StopBitsComboBox.SelectedItem).Value,
                        delegate
                        {
                            Dispatcher.Invoke(LoadReceivedData);
                        });
                }
                catch (Exception exception)
                {
                    InternalLogger.Log.Error("Cannot open port:", exception);
                    ShowErrorBox(@"Cannot open selected port with selected configuration");
                }
            }
        }

        /// <summary>
        /// Show error box with message
        /// </summary>
        /// <param name="errorText">Message to show</param>
        private static void ShowErrorBox(string errorText)
        {
            MessageBox.Show(errorText, ErrorMessageBoxHeader, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// Refresh port list
        /// </summary>
        /// <param name="sender">Not used</param>
        /// <param name="e">Not used</param>
        private void PortsRefresh(object sender, EventArgs e)
        {
            Ports.Clear();
            foreach (var port in SerialPortCommunicator.Ports)
            {
                Ports.Add(port);
            }
        }

        /// <summary>
        /// Load received data from serial port
        /// </summary>
        private void LoadReceivedData()
        {
            try
            {
                string s;
                do
                {
                    try
                    {
                        s = _serialPortCommunicator.ReadExisting();
                        OutputTextBox.AppendText(s);
                    }
                    catch (CannotFindStartSymbolException)
                    {
                        break;
                    }
                } while (s != "");
            }
            catch (Exception exception)
            {
                InternalLogger.Log.Error("Cannot read data from port:", exception);
                ShowErrorBox(exception.Message);
            }
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }

        /// <summary>
        /// Send text to port
        /// </summary>
        /// <param name="sender">Not used</param>
        /// <param name="e">Not used</param>
        private void SendText(object sender, EventArgs e)
        {
            try
            {
                if (InputTextBox.Text != "")
                {
                    _serialPortCommunicator.Send(InputTextBox.Text);
                    InputTextBox.Text = "";
                }
            }
            catch (Exception exception)
            {
                InternalLogger.Log.Error(@"Cannot write to port", exception);
                ShowErrorBox(@"Cannot write to port");
            }
        }
    }
}