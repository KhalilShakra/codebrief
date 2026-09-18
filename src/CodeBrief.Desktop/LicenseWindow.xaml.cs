using System.Windows;
using CodeBrief.Licensing;

namespace CodeBrief.Desktop;

public partial class LicenseWindow : Window
{
    private readonly UiState _ui;

    public LicenseWindow(UiState ui)
    {
        _ui = ui;
        InitializeComponent();
        KeyBox.Text = ui.LicenseKey ?? "";
        RefreshStatus();
    }

    private void RefreshStatus()
    {
        LicenseGate.CanAnalyze(_ui, out var message);
        var license = LicenseGate.Current(_ui);
        StatusText.Text = license.IsValid
            ? $"Aktiv licens för {license.Payload?.Name} ({license.Payload?.Edition}) till {license.Payload?.ExpiresUtc:yyyy-MM-dd}."
            : message;
    }

    private void OnActivate(object sender, RoutedEventArgs e)
    {
        var result = LicenseCrypt.Verify(KeyBox.Text, LicensePublicKey.Pem);
        if (!result.IsValid)
        {
            MessageBox.Show(result.Message, "CodeBrief", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        _ui.LicenseKey = KeyBox.Text.Trim();
        AppSettings.Save(_ui);
        RefreshStatus();
        MessageBox.Show("Licensen är aktiverad.", "CodeBrief", MessageBoxButton.OK, MessageBoxImage.Information);
        DialogResult = true;
        Close();
    }

    private void OnClose(object sender, RoutedEventArgs e) => Close();
}
