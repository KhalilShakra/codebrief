using System.IO;
using System.Windows;
using System.Windows.Threading;

namespace CodeBrief.Desktop;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        DispatcherUnhandledException += OnDispatcherUnhandledException;
        AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
        base.OnStartup(e);
    }

    private static void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        ShowAndLog(e.Exception);
        e.Handled = true;
    }

    private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        if (e.ExceptionObject is Exception ex)
        {
            ShowAndLog(ex);
        }
    }

    private static void ShowAndLog(Exception ex)
    {
        try
        {
            var path = Path.Combine(Path.GetTempPath(), "codebrief-error.log");
            File.AppendAllText(path, $"[{DateTime.Now:u}] {ex}{Environment.NewLine}");
        }
        catch
        {
            // ignore logging failures
        }

        MessageBox.Show(
            "CodeBrief stötte på ett fel:\n\n" + ex.Message,
            "CodeBrief",
            MessageBoxButton.OK,
            MessageBoxImage.Error);
    }
}
