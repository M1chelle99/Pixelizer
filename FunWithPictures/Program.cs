using System;
using System.IO;
using System.Windows.Forms;

namespace FunWithPictures
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (args.Length != 1) ShowError("Please provide an png picture in the launch arguments.");
            if (!File.Exists(args[0])) ShowError("Cannot find file at the provided filepath.");

            Application.Run(new WinMain(args));
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            ShowError($"An unexpected error occured.\n\n{e.ExceptionObject}");
        }

        private static void ShowError(string text)
        {
            MessageBox.Show(
                caption: "Error",
                text: text,
                icon: MessageBoxIcon.Error,
                buttons: MessageBoxButtons.OK);
        }
    }
}
