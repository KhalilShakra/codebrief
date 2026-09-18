using System.IO;
using System.Text.Json;

namespace CodeBrief.Desktop;

public sealed class UiState
{
    public double Left { get; set; } = double.NaN;
    public double Top { get; set; } = double.NaN;
    public double Width { get; set; } = 1280;
    public double Height { get; set; } = 860;
    public bool Maximized { get; set; }
    public string? LastFolder { get; set; }
    public string Provider { get; set; } = "Local";
    public string? EncryptedApiKey { get; set; }
    public string OpenAiModel { get; set; } = "gpt-4.1-mini";
    public string AnthropicModel { get; set; } = "claude-sonnet-4-5";
    public string? LicenseKey { get; set; }
    public int TrialUsed { get; set; }
}

internal static class AppSettings
{
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    public static string FilePath { get; } = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "CodeBrief",
        "ui.json");

    public static UiState Load()
    {
        try
        {
            if (File.Exists(FilePath))
            {
                return JsonSerializer.Deserialize<UiState>(File.ReadAllText(FilePath)) ?? new UiState();
            }
        }
        catch
        {
            // keep defaults
        }

        return new UiState();
    }

    public static void Save(UiState state)
    {
        try
        {
            var dir = Path.GetDirectoryName(FilePath);
            if (!string.IsNullOrEmpty(dir))
            {
                Directory.CreateDirectory(dir);
            }

            File.WriteAllText(FilePath, JsonSerializer.Serialize(state, JsonOptions));
        }
        catch
        {
            // ignore
        }
    }
}
