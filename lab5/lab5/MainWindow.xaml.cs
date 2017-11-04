using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO.Ports;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using lab5.Backend;
using lab5.Backend.Exceptions;
using lab5.Enums;
using lab5.Enums.Utilities;
using lab5.ViewModels;

namespace lab5
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string ErrorMessageBoxHeader = @"Oops, we have an error";
        private const string ErrorLoadingVm = "Cannot connect viewModel to MainWindow";
        private const int DefaultNumValue = 0;
        private readonly MainWindowWm _vm;

        public MainWindow()
        {
            InitializeComponent();
            _vm = DataContext as MainWindowWm; //Get VM from view's DataContext
            if (_vm == null)
            {
                InternalLogger.Log.Debug(ErrorLoadingVm);
                ShowErrorBox(ErrorLoadingVm);
            }
        }

        private void StartStopButtonPressed(object sender, RoutedEventArgs e)
        {
            if (_vm.IsPortOpen)
            {
                _vm.CloseConnection();
            }
            else
            {
                try
                {
                    _vm.OpenConnection();
                }
                catch (Exception exception)
                {
                    InternalLogger.Log.Error("Cannot open port:", exception);
                    ShowErrorBox(@"Cannot open selected port with selected configuration");
                }
            }
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
                _vm.SendText();
            }
            catch (Exception exception)
            {
                InternalLogger.Log.Error($"Cannot write to port with target ID \"{_vm.DestinationId}\"", exception);
                ShowErrorBox(@"Cannot write to port. Maybe you write bad target ID");
            }
        }

        /// <summary>
        /// Refresh port list
        /// </summary>
        /// <param name="sender">Not used</param>
        /// <param name="e">Not used</param>
        private void PortsRefresh(object sender, EventArgs e)
        {
            _vm.PortsRefresh();
        }

        /// <summary>
        /// Close port with window
        /// </summary>
        /// <param name="sender">Not used</param>
        /// <param name="e">Not used</param>
        private void OnWindowClosed(object sender, EventArgs e)
        {
            _vm.CloseConnection();
        }

        /// <summary>
        /// Automatic scrolling window to bottom
        /// </summary>
        /// <param name="sender">Not used</param>
        /// <param name="e">Not used</param>
        private void OnOutputTextChanged(object sender, EventArgs e)
        {
            OutputTextBox.ScrollToEnd();
        }

        /// <summary>
        /// Automatic blocking input non-number symbols
        /// </summary>
        /// <param name="sender">Not used</param>
        /// <param name="e">Object to handle</param>
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        /// <summary>
        /// Show error box with message
        /// </summary>
        /// <param name="errorText">Message to show</param>
        public static void ShowErrorBox(string errorText)
        {
            MessageBox.Show(errorText, ErrorMessageBoxHeader, MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}