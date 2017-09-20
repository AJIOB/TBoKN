using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TOKS_lab1.backend;
using TOKS_lab1.Enums;

namespace TOKS_lab1
{
    public partial class MainWindow : Form
    {
        private readonly SerialPortCommunicator _serialPortCommunicator = new SerialPortCommunicator();
        private String startString = "Start";
        private String stopString = "Stop";

        public MainWindow()
        {
            InitializeComponent();
            DynamicConfigComponents();
        }

        private void DynamicConfigComponents()
        {
            RefreshViewAsRequred();

            foreach (var name in Enum.GetValues(typeof(EBaudrate)))
            {
                baudrateComboBox.Items.Add((int) name);
            }
            baudrateComboBox.SelectedIndex = 1;

            foreach (var name in Enum.GetValues(typeof(EDataBits)))
            {
                dataBitsComboBox.Items.Add((int) name);
            }
            dataBitsComboBox.SelectedIndex = 3;

            foreach (var name in Enum.GetValues(typeof(StopBits)))
            {
                if (!name.Equals(StopBits.None))
                {
                    stopBitsComboBox.Items.Add(name);
                }
            }
            stopBitsComboBox.SelectedIndex = 0;

            foreach (var name in Enum.GetValues(typeof(Parity)))
            {
                parityComboBox.Items.Add(name);
            }
            parityComboBox.SelectedIndex = 0;

            foreach (var name in Enum.GetValues(typeof(Parity)))
            {
                flowControlComboBox.Items.Add(name);
            }
            flowControlComboBox.SelectedIndex = 0;

            FormClosed += (sender, e) =>
            {
                if (_serialPortCommunicator.IsOpen)
                {
                    _serialPortCommunicator.Close();
                }
            };
            currentPortComboBox.DropDown += (sender, args) =>
            {
                RefreshPortList();
            };
        }

        private void RefreshPortList()
        {
            currentPortComboBox.Items.Clear();
            foreach (var s in SerialPort.GetPortNames())
            {
                currentPortComboBox.Items.Add(s);
            }
        }

        private void inputTextBox_TextChanged(object sender, EventArgs e)
        {
            try
            {
                _serialPortCommunicator.Send(inputTextBox.Text);
                inputTextBox.Text = "";
            }
            catch
            {
                ShowErrorBox(@"Cannot write to port");
            }
        }

        private void startStopButton_Click(object sender, EventArgs e)
        {
            if (_serialPortCommunicator.IsOpen)
            {
                _serialPortCommunicator.Close();
            }
            else
            {
                try
                {
                    _serialPortCommunicator.Open((string) currentPortComboBox.SelectedItem,
                        (EBaudrate) baudrateComboBox.SelectedItem, (Parity) parityComboBox.SelectedItem,
                        (EDataBits) dataBitsComboBox.SelectedItem, (StopBits) stopBitsComboBox.SelectedItem);
                    serialPort.DataReceived += delegate
                    {
                        this.Invoke((MethodInvoker) (delegate() { outputTextBox.AppendText(_serialPortCommunicator.ReadExisting()); }));
                    };
                }
                catch
                {
                    ShowErrorBox(@"Cannot open port with selected mode");
                }
            }

            RefreshViewAsRequred();
        }

        private void RefreshViewAsRequred()
        {
            bool isStarted = _serialPortCommunicator.IsOpen;
            currentPortComboBox.Enabled = !isStarted;
            baudrateComboBox.Enabled = !isStarted;
            dataBitsComboBox.Enabled = !isStarted;
            stopBitsComboBox.Enabled = !isStarted;
            parityComboBox.Enabled = !isStarted;
            flowControlComboBox.Enabled = false;
            inputTextBox.Enabled = isStarted;

            startStopButton.Text = (isStarted ? stopString : startString);
        }

        private void ShowErrorBox(String errorText)
        {
            MessageBox.Show(errorText, @"Oops, we have an error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }
}