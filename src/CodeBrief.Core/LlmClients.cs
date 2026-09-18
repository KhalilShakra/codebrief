using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace CodeBrief.Core;

public interface ILlmClient
{
    string ProviderName { get; }

    Task<string> CompleteJsonAsync(string systemPrompt, string userPrompt, CancellationToken cancellationToken);
}

public sealed class OpenAiClient : ILlmClient
{
    private readonly HttpClient _http;
    private readonly string _apiKey;
    private readonly string _model;

    public OpenAiClient(HttpClient http, string apiKey, string model)
    {
        _http = http;
        _apiKey = apiKey;
        _model = string.IsNullOrWhiteSpace(model) ? "gpt-4.1-mini" : model;
    }

    public string ProviderName => "OpenAI " + _model;

    public async Task<string> CompleteJsonAsync(string systemPrompt, string userPrompt, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
        var body = new
        {
            model = _model,
            temperature = 0.2,
            response_format = new { type = "json_object" },
            messages = new object[]
            {
                new { role = "system", content = systemPrompt },
                new { role = "user", content = userPrompt }
            }
        };
        request.Content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");

        using var response = await _http.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var json = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException($"OpenAI {response.StatusCode}: {Trim(json)}");
        }

        using var doc = JsonDocument.Parse(json);
        return doc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString()
               ?? throw new InvalidOperationException("Tomt svar från OpenAI.");
    }

    private static string Trim(string text) => text.Length <= 400 ? text : text[..400];
}

public sealed class AnthropicClient : ILlmClient
{
    private readonly HttpClient _http;
    private readonly string _apiKey;
    private readonly string _model;

    public AnthropicClient(HttpClient http, string apiKey, string model)
    {
        _http = http;
        _apiKey = apiKey;
        _model = string.IsNullOrWhiteSpace(model) ? "claude-sonnet-4-5" : model;
    }

    public string ProviderName => "Anthropic " + _model;

    public async Task<string> CompleteJsonAsync(string systemPrompt, string userPrompt, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "https://api.anthropic.com/v1/messages");
        request.Headers.Add("x-api-key", _apiKey);
        request.Headers.Add("anthropic-version", "2023-06-01");
        var body = new
        {
            model = _model,
            max_tokens = 4000,
            temperature = 0.2,
            system = systemPrompt,
            messages = new object[] { new { role = "user", content = userPrompt } }
        };
        request.Content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");

        using var response = await _http.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var json = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException($"Anthropic {response.StatusCode}: {Trim(json)}");
        }

        using var doc = JsonDocument.Parse(json);
        foreach (var block in doc.RootElement.GetProperty("content").EnumerateArray())
        {
            if (block.TryGetProperty("text", out var text))
            {
                return text.GetString() ?? "";
            }
        }

        throw new InvalidOperationException("Tomt svar från Anthropic.");
    }

    private static string Trim(string text) => text.Length <= 400 ? text : text[..400];
}
