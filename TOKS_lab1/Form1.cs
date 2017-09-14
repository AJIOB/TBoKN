using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
            RefreshPortList();
            RefreshViewAsRequred();
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
                //TODO
            }
            else
            {
                //TODO
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
            flowControlComboBox.Enabled = !isStarted;
            inputTextBox.Enabled = isStarted;
            
            startStopButton.Text = (isStarted ? stopString : startString);
        }
    }
}
