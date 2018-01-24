using DeninaSharp.Core;
using System;
using System.Windows.Forms;

namespace UI
{
    internal static class Program
    {
        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Pipeline.SetGlobalVariable("Http.AllowedDomains", "*");

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainWindow());
        }
    }
}