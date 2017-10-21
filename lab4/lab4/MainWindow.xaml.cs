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
                    _serialPortCommunicator.Open((string)CurrentPortComboBox.SelectedItem,
                        (EBaudrate)BaudrateComboBox.SelectedItem, (Parity)ParityComboBox.SelectedItem,
                        (EDataBits)DataBitsComboBox.SelectedItem, (StopBits)StopBitsComboBox.SelectedItem,
                        //TODO
                        /*delegate
                        {
                            try
                            {
                                this.Invoke((MethodInvoker)(delegate ()
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
                                }));
                            }
                            catch (Exception exception)
                            {
                                ShowErrorBox(exception.Message);
                            }
                        }*/null);
                }
                catch
                {
                    //ShowErrorBox(@"Cannot open selected port with selected configuration");
                }
            }
        }
    }
}
