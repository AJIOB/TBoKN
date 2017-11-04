using System.Windows;
using lab5.Backend;

namespace lab5
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void OnAppStarted(object sender, StartupEventArgs e)
        {
            InternalLogger.Log.Info("Application was started");
        }

        private void OnAppClosed(object sender, ExitEventArgs exitEventArgs)
        {
            InternalLogger.Log.Info("Application was finished");
        }
    }
}
