namespace Pixelizer;

internal static class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

        Application.SetHighDpiMode(HighDpiMode.SystemAware);
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        if (args.Length != 1) ShowError("Please provide a picture in the launch arguments by drag and dropping an image on this executable.");
        if (!File.Exists(args[0])) ShowError("File not found at the provided filepath.");

        Application.Run(new WinMain(args));
    }

    private static void CurrentDomain_UnhandledException(object _, UnhandledExceptionEventArgs e)
    {
        ShowError($"An unexpected error occured.\n\n{e.ExceptionObject}");
    }

    private static void ShowError(string text)
    {
        MessageBox.Show(
            caption: @"Error",
            text: text,
            icon: MessageBoxIcon.Error,
            buttons: MessageBoxButtons.OK);
    }
}