using CodeBrief.Contracts;

namespace CodeBrief.Core;

public sealed class LlmCodeAnalyzer : ICodeAnalyzer
{
    private readonly ILlmClient _client;
    private readonly ICodeAnalyzer _fallback;

    public LlmCodeAnalyzer(ILlmClient client, ICodeAnalyzer fallback)
    {
        _client = client;
        _fallback = fallback;
    }

    public string Name => _client.ProviderName;

    public async Task<AnalysisReport> AnalyzeAsync(AnalysisRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var json = await _client
                .CompleteJsonAsync(AnalysisPrompts.System, AnalysisPrompts.User(request), cancellationToken)
                .ConfigureAwait(false);
            return ReportJsonParser.Parse(json, _client.ProviderName);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            var fallback = await _fallback.AnalyzeAsync(request, cancellationToken).ConfigureAwait(false);
            fallback.Analyzer = $"{_fallback.Name} (AI-fel: {ex.Message})";
            fallback.Overview = new OverviewSection
            {
                Summary = $"AI-analysen misslyckades ({ex.Message}). Rapporten nedan är den lokala reservanalysen. {fallback.Overview.Summary}"
            };
            return fallback;
        }
    }
}
