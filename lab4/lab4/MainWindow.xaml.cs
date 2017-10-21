using System;
using System.Collections.Generic;
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

namespace lab4
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string ErrorMessageBoxHeader = @"Oops, we have an error";

        private readonly SerialPortCommunicator _serialPortCommunicator = new SerialPortCommunicator();

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
                    _serialPortCommunicator.Open((string) CurrentPortComboBox.SelectedItem,
                        (EBaudrate) BaudrateComboBox.SelectedItem, (Parity) ParityComboBox.SelectedItem,
                        (EDataBits) DataBitsComboBox.SelectedItem, (StopBits) StopBitsComboBox.SelectedItem,
                        delegate
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
                        });
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
    }
}