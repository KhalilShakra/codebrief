using System.Windows;
using System.Windows.Controls;

namespace CodeBrief.Desktop;

public partial class SettingsWindow : Window
{
    private readonly UiState _ui;

    public SettingsWindow(UiState ui)
    {
        _ui = ui;
        InitializeComponent();
        SelectProvider(ui.Provider);
        OpenAiModelBox.Text = ui.OpenAiModel;
        AnthropicModelBox.Text = ui.AnthropicModel;
        var existing = SecretStore.Unprotect(ui.EncryptedApiKey);
        if (existing.Length > 0)
        {
            ApiKeyBox.Password = existing;
        }
    }

    private void SelectProvider(string provider)
    {
        foreach (ComboBoxItem item in ProviderBox.Items)
        {
            if (string.Equals(item.Tag as string, provider, StringComparison.OrdinalIgnoreCase))
            {
                ProviderBox.SelectedItem = item;
                return;
            }
        }

        ProviderBox.SelectedIndex = 0;
    }

    private void OnSave(object sender, RoutedEventArgs e)
    {
        _ui.Provider = (ProviderBox.SelectedItem as ComboBoxItem)?.Tag as string ?? "Local";
        _ui.OpenAiModel = OpenAiModelBox.Text.Trim();
        _ui.AnthropicModel = AnthropicModelBox.Text.Trim();
        if (!string.IsNullOrWhiteSpace(ApiKeyBox.Password))
        {
            _ui.EncryptedApiKey = SecretStore.Protect(ApiKeyBox.Password.Trim());
        }

        AppSettings.Save(_ui);
        DialogResult = true;
        Close();
    }

    private void OnCancel(object sender, RoutedEventArgs e) => Close();
}
