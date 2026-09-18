using System.Net.Http;
using CodeBrief.Core;

namespace CodeBrief.Desktop;

internal static class HttpClients
{
    public static HttpClient Shared { get; } = new()
    {
        Timeout = TimeSpan.FromMinutes(2)
    };
}

internal static class AnalyzerFactory
{
    public static ICodeAnalyzer Create(UiState ui)
    {
        var fallback = new HeuristicCodeAnalyzer();
        var key = SecretStore.Unprotect(ui.EncryptedApiKey);
        if (string.IsNullOrWhiteSpace(key))
        {
            return fallback;
        }

        return ui.Provider switch
        {
            "OpenAI" => new LlmCodeAnalyzer(new OpenAiClient(HttpClients.Shared, key, ui.OpenAiModel), fallback),
            "Anthropic" => new LlmCodeAnalyzer(new AnthropicClient(HttpClients.Shared, key, ui.AnthropicModel), fallback),
            _ => fallback
        };
    }
}
