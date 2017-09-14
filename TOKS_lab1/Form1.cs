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
using TOKS_lab1.Enums;

namespace TOKS_lab1
{
    public partial class MainWindow : Form
    {
        private SerialPort serialPort = null;
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
                if (serialPort != null)
                {
                    startStopButton_Click(sender, e);
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
            serialPort.Write(inputTextBox.Text);
            inputTextBox.Text = "";
        }

        private void startStopButton_Click(object sender, EventArgs e)
        {
            if (serialPort != null)
            {
                serialPort.Close();
                serialPort = null;
            }
            else
            {
                //TODO
                try
                {
                    serialPort = new SerialPort((String) currentPortComboBox.SelectedItem,
                        (int) (EBaudrate) baudrateComboBox.SelectedItem, (Parity) parityComboBox.SelectedItem,
                        (int) (EDataBits) dataBitsComboBox.SelectedItem, (StopBits) stopBitsComboBox.SelectedItem);
                    serialPort.Open();
                }
                catch
                {
                    MessageBox.Show(@"Cannot open port with selected mode", @"Oops, we have an error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    serialPort = null;
                }
            }

            RefreshViewAsRequred();
        }

        private void RefreshViewAsRequred()
        {
            bool isStarted = (serialPort != null);
            currentPortComboBox.Enabled = !isStarted;
            baudrateComboBox.Enabled = !isStarted;
            dataBitsComboBox.Enabled = !isStarted;
            stopBitsComboBox.Enabled = !isStarted;
            parityComboBox.Enabled = !isStarted;
            flowControlComboBox.Enabled = false; //TODO: fix
            inputTextBox.Enabled = isStarted;

            startStopButton.Text = (isStarted ? stopString : startString);
        }
    }
}