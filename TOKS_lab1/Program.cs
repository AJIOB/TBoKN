using System;
using System.Windows.Forms;
using TOKS_lab1.backend;

namespace TOKS_lab1
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            InternalLogger.Log.Info("Application was started");
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainWindow());
            InternalLogger.Log.Info("Application was finished");
        }
    }
}
