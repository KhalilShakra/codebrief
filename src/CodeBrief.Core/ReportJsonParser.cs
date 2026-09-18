using System.Text.Json;
using CodeBrief.Contracts;

namespace CodeBrief.Core;

public static class ReportJsonParser
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public static AnalysisReport Parse(string raw, string analyzerName)
    {
        var json = StripFence(raw);
        var report = JsonSerializer.Deserialize<AnalysisReport>(json, Options)
                     ?? throw new InvalidOperationException("AI-svaret var tomt.");
        report.Analyzer = analyzerName;
        report.GeneratedAt = DateTimeOffset.Now;
        report.Overview ??= new OverviewSection();
        report.Pseudocode ??= new PseudocodeSection();
        report.Flow ??= new FlowSection();
        report.Issues ??= new IssuesSection();
        report.Improvements ??= new ImprovementsSection();
        report.Fix ??= new FixSection();
        report.Language ??= new LanguageInfo();
        return report;
    }

    public static string StripFence(string raw)
    {
        var text = raw.Trim();
        if (!text.StartsWith("```", StringComparison.Ordinal))
        {
            return text;
        }

        var firstLine = text.IndexOf('\n');
        if (firstLine < 0)
        {
            return text;
        }

        text = text[(firstLine + 1)..];
        var end = text.LastIndexOf("```", StringComparison.Ordinal);
        return end >= 0 ? text[..end].Trim() : text.Trim();
    }
}
