using CodeBrief.Contracts;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace CodeBrief.Core;

public static class PdfReportWriter
{
    public static void Write(AnalysisReport report, string path)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(40);
                page.Size(PageSizes.A4);
                page.DefaultTextStyle(x => x.FontSize(10).FontColor(Colors.BlueGrey.Darken4));

                page.Header().Column(col =>
                {
                    col.Item().Text("CodeBrief").FontSize(18).SemiBold().FontColor(Color.FromHex("4C8DFF"));
                    col.Item().Text($"{report.Language.Name} · {report.Analyzer} · {report.GeneratedAt:yyyy-MM-dd HH:mm}")
                        .FontSize(9).FontColor(Colors.Grey.Darken1);
                    col.Item().PaddingTop(8).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                });

                page.Content().PaddingTop(16).Column(col =>
                {
                    col.Spacing(12);
                    Section(col, "1. Språk och översikt", report.Overview.Summary);
                    Section(col, "2. Pseudokod", string.Join('\n', report.Pseudocode.Lines));
                    Section(col, "3. Kodflöde",
                        "Indata\n" + Lines(report.Flow.Inputs) +
                        "\nSteg\n" + Lines(report.Flow.Steps) +
                        "\nUtdata\n" + Lines(report.Flow.Outputs));
                    Section(col, "4. Buggar, fel och säkerhet", FormatIssues(report));
                    Section(col, "5. Prestanda och best practices",
                        Lines(report.Improvements.Performance) + "\n" + Lines(report.Improvements.Maintainability));
                    Section(col, "6. Korrigerad kod",
                        report.Fix.Explanation + (string.IsNullOrWhiteSpace(report.Fix.CorrectedCode)
                            ? ""
                            : "\n\n" + report.Fix.CorrectedCode));
                });

                page.Footer().AlignRight().Text(text =>
                {
                    text.Span("Sida ");
                    text.CurrentPageNumber();
                    text.Span(" / ");
                    text.TotalPages();
                });
            });
        }).GeneratePdf(path);
    }

    private static void Section(ColumnDescriptor col, string title, string body)
    {
        col.Item().Text(title).SemiBold().FontSize(12).FontColor(Color.FromHex("0E1116"));
        col.Item().Text(string.IsNullOrWhiteSpace(body) ? "—" : body).FontSize(10);
    }

    private static string Lines(IReadOnlyList<string> items) =>
        items.Count == 0 ? "—" : string.Join('\n', items.Select(x => "• " + x));

    private static string FormatIssues(AnalysisReport report)
    {
        var bits = new List<string>();
        if (report.Issues.IsCorrect)
        {
            bits.Add("Koden ser korrekt ut enligt den här analysen.");
        }

        bits.AddRange(report.Issues.Bugs.Select(f => $"[BUG {f.Severity}] {f.Title}: {f.Detail}"));
        bits.AddRange(report.Issues.EdgeCases.Select(f => $"[KANT {f.Severity}] {f.Title}: {f.Detail}"));
        bits.AddRange(report.Issues.Security.Select(f => $"[SÄK {f.Severity}] {f.Title}: {f.Detail}"));
        return bits.Count == 0 ? "Inga anmärkningar." : string.Join('\n', bits);
    }
}
