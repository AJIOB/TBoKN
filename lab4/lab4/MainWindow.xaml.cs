using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using lab4.backend;
using lab4.backend.exceptions;
using lab4.Enums;
using lab4.Enums.Utilities;

namespace lab4
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string ErrorMessageBoxHeader = @"Oops, we have an error";

        private readonly SerialPortCommunicator _serialPortCommunicator = new SerialPortCommunicator();

        public ObservableCollection<string> Ports { get; } = new ObservableCollection<string>();

        public MainWindow()
        {
            InitializeComponent();
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
                        delegate { LoadReceivedData(); });
                }
                catch (Exception exception)
                {
                    InternalLogger.Log.Error("Caanot open port:", exception);
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
    }
}