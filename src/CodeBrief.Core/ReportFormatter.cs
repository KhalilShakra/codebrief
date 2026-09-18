using System.Text;
using CodeBrief.Contracts;

namespace CodeBrief.Core;

public static class ReportFormatter
{
    public static string ToMarkdown(AnalysisReport report)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"# CodeBrief-rapport — {report.Language.Name}");
        sb.AppendLine();
        sb.AppendLine($"Komplexitet: {report.Language.Complexity} · Analys: {report.Analyzer} · {report.GeneratedAt:yyyy-MM-dd HH:mm}");
        if (report.Language.Frameworks.Count > 0)
        {
            sb.AppendLine("Ramverk: " + string.Join(", ", report.Language.Frameworks));
        }

        sb.AppendLine();
        sb.AppendLine("## 1. Språk och översikt");
        sb.AppendLine(report.Overview.Summary);

        sb.AppendLine();
        sb.AppendLine("## 2. Pseudokod");
        sb.AppendLine("```");
        foreach (var line in report.Pseudocode.Lines)
        {
            sb.AppendLine(line);
        }
        sb.AppendLine("```");

        sb.AppendLine();
        sb.AppendLine("## 3. Kodflöde");
        sb.AppendLine("### Indata");
        foreach (var x in report.Flow.Inputs) sb.AppendLine("- " + x);
        sb.AppendLine("### Steg");
        foreach (var x in report.Flow.Steps) sb.AppendLine("- " + x);
        sb.AppendLine("### Utdata");
        foreach (var x in report.Flow.Outputs) sb.AppendLine("- " + x);

        sb.AppendLine();
        sb.AppendLine("## 4. Buggar, fel och säkerhet");
        if (report.Issues.IsCorrect)
        {
            sb.AppendLine("Koden ser korrekt ut enligt den här analysen.");
        }

        WriteFindings(sb, "Buggar", report.Issues.Bugs);
        WriteFindings(sb, "Kantfall", report.Issues.EdgeCases);
        WriteFindings(sb, "Säkerhet", report.Issues.Security);

        sb.AppendLine();
        sb.AppendLine("## 5. Prestanda och best practices");
        foreach (var x in report.Improvements.Performance) sb.AppendLine("- " + x);
        foreach (var x in report.Improvements.Maintainability) sb.AppendLine("- " + x);

        sb.AppendLine();
        sb.AppendLine("## 6. Korrigerad kod");
        if (!report.Fix.Needed || string.IsNullOrWhiteSpace(report.Fix.CorrectedCode))
        {
            sb.AppendLine(report.Fix.Explanation);
        }
        else
        {
            sb.AppendLine(report.Fix.Explanation);
            sb.AppendLine();
            sb.AppendLine("```");
            sb.AppendLine(report.Fix.CorrectedCode);
            sb.AppendLine("```");
        }

        return sb.ToString();
    }

    private static void WriteFindings(StringBuilder sb, string heading, IReadOnlyList<Finding> items)
    {
        sb.AppendLine($"### {heading}");
        if (items.Count == 0)
        {
            sb.AppendLine("Inga.");
            return;
        }

        foreach (var item in items)
        {
            sb.AppendLine($"- **{item.Title}** ({item.Severity}): {item.Detail}");
        }
    }
}
