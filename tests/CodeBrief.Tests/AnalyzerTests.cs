using CodeBrief.Contracts;
using CodeBrief.Core;
using CodeBrief.Licensing;

namespace CodeBrief.Tests;

public class LanguageDetectorTests
{
    [Fact]
    public void Detects_csharp_sample()
    {
        var code = """
            using System;
            namespace Demo {
              public class Program {
                public static void Main() { Console.WriteLine("hi"); }
              }
            }
            """;

        var guess = LanguageDetector.Detect(code, "Program.cs");
        Assert.Equal("csharp", guess.Id);
        Assert.True(guess.Confidence > 0.4);
    }

    [Fact]
    public void Detects_python_from_def()
    {
        var guess = LanguageDetector.Detect("def hello(name):\n    print(name)\n", "app.py");
        Assert.Equal("python", guess.Id);
    }
}

public class HeuristicAnalyzerTests
{
    [Fact]
    public async Task Flags_hardcoded_secret_and_division()
    {
        var analyzer = new HeuristicCodeAnalyzer();
        var report = await analyzer.AnalyzeAsync(new AnalysisRequest
        {
            Source = """
                using System;
                class Program {
                  static string password = "admin123";
                  static void Main() {
                    foreach (var n in new[] { 10, 0 }) Console.WriteLine(100 / n);
                  }
                }
                """,
            FileName = "Program.cs"
        });

        Assert.Equal("csharp", report.Language.Id);
        Assert.Contains(report.Issues.Security, x => x.Title.Contains("Hemlighet", StringComparison.OrdinalIgnoreCase) || x.Title.Contains("hemlighet", StringComparison.OrdinalIgnoreCase));
        Assert.True(report.Issues.Bugs.Count + report.Issues.Security.Count > 0);
        Assert.False(string.IsNullOrWhiteSpace(ReportFormatter.ToMarkdown(report)));
    }
}

public class ReportJsonParserTests
{
    [Fact]
    public void Parses_fenced_json()
    {
        var json = """
            ```json
            {"language":{"id":"python","name":"Python","frameworks":[],"complexity":"low"},"section_1_overview":{"summary":"En liten funktion."},"section_2_pseudocode":{"lines":["FUNKTION hello"]},"section_3_flow":{"inputs":["namn"],"steps":["kör"],"outputs":["text"]},"section_4_issues":{"is_correct":true,"bugs":[],"edge_cases":[],"security":[]},"section_5_improvements":{"performance":["ok"],"maintainability":["ok"]},"section_6_fix":{"needed":false,"explanation":"Ingen rättning.","corrected_code":null}}
            ```
            """;

        var report = ReportJsonParser.Parse(json, "test");
        Assert.Equal("Python", report.Language.Name);
        Assert.Equal("test", report.Analyzer);
        Assert.Equal("En liten funktion.", report.Overview.Summary);
    }
}

public class LicenseTests
{
    [Fact]
    public void Issues_and_verifies_roundtrip()
    {
        var (pub, priv) = LicenseCrypt.CreateKeyPair();
        var key = LicenseCrypt.Issue(new LicensePayload
        {
            Name = "Testdotter",
            Email = "test@example.com",
            Edition = "pro",
            ExpiresUtc = DateTimeOffset.UtcNow.AddDays(30)
        }, priv);

        var result = LicenseCrypt.Verify(key, pub);
        Assert.True(result.IsValid);
        Assert.Equal("Testdotter", result.Payload?.Name);
    }

    [Fact]
    public void Rejects_tampered_key()
    {
        var (pub, priv) = LicenseCrypt.CreateKeyPair();
        var key = LicenseCrypt.Issue(new LicensePayload
        {
            Name = "A",
            ExpiresUtc = DateTimeOffset.UtcNow.AddDays(10)
        }, priv);

        var result = LicenseCrypt.Verify(key + "x", pub);
        Assert.False(result.IsValid);
    }

    [Fact]
    public void Rejects_expired_license()
    {
        var (pub, priv) = LicenseCrypt.CreateKeyPair();
        var key = LicenseCrypt.Issue(new LicensePayload
        {
            Name = "Gammal",
            ExpiresUtc = DateTimeOffset.UtcNow.AddDays(-1)
        }, priv);

        var result = LicenseCrypt.Verify(key, pub);
        Assert.False(result.IsValid);
        Assert.Contains("gått ut", result.Message, StringComparison.OrdinalIgnoreCase);
    }
}

public class PdfReportWriterTests
{
    [Fact]
    public async Task Writes_pdf_file()
    {
        var analyzer = new HeuristicCodeAnalyzer();
        var report = await analyzer.AnalyzeAsync(new AnalysisRequest
        {
            Source = "print('hello')",
            FileName = "hi.py"
        });
        var path = Path.Combine(Path.GetTempPath(), "codebrief-test.pdf");
        PdfReportWriter.Write(report, path);
        Assert.True(File.Exists(path));
        Assert.True(new FileInfo(path).Length > 200);
    }
}
