using System.Text.RegularExpressions;
using CodeBrief.Contracts;

namespace CodeBrief.Core;

public static class LanguageDetector
{
    private sealed record Profile(
        string Id,
        string Name,
        string[] Extensions,
        string[] Strong,
        string[] Keywords,
        (string Needle, string Label)[] Frameworks);

    private static readonly Profile[] Profiles =
    [
        new("csharp", "C#", [".cs", ".csx"],
            [@"\bnamespace\s+\w+", @"\busing\s+System\b", @"\bConsole\.(Write|WriteLine)\b", @"\basync\s+Task\b", @"\brecord\s+(class|struct)?\s*\w+"],
            ["public class", "var ", "get;", "init;", "Linq", "await ", "string?", "partial class"],
            [("Microsoft.AspNetCore", "ASP.NET Core"), ("UnityEngine", "Unity"), ("System.Windows", "WPF"),
             ("Microsoft.EntityFrameworkCore", "Entity Framework"), ("Xunit", "xUnit")]),

        new("python", "Python", [".py", ".pyw"],
            [@"^\s*def\s+\w+\s*\(", @"^\s*elif\s+", @"^\s*from\s+\w+\s+import\b", @"\bNone\b", @"\bself\."],
            ["print(", "lambda ", "if __name__", "True", "False", "yield ", "with "],
            [("flask", "Flask"), ("django", "Django"), ("fastapi", "FastAPI"), ("pandas", "pandas"),
             ("numpy", "NumPy"), ("torch", "PyTorch"), ("tensorflow", "TensorFlow")]),

        new("cpp", "C++", [".cpp", ".cc", ".cxx", ".hpp", ".hxx", ".h++"],
            [@"#include\s*<iostream>", @"\bstd::", @"\bcout\s*<<", @"\bnullptr\b", @"\btemplate\s*<"],
            ["int main", "using namespace", "class ", "virtual ", "override"],
            [("Qt", "Qt"), ("opencv", "OpenCV"), ("boost", "Boost")]),

        new("c", "C", [".c", ".h"],
            [@"#include\s*<stdio.h>", @"\bprintf\s*\(", @"\bmalloc\s*\(", @"\btypedef\s+struct\b"],
            ["int main", "NULL", "sizeof"],
            []),

        new("java", "Java", [".java"],
            [@"\bpublic\s+class\s+\w+", @"\bSystem\.out\.print", @"\bpublic\s+static\s+void\s+main\s*\(", @"\bpackage\s+[\w.]+;"],
            ["import java.", "@Override", "ArrayList", "throws "],
            [("org.springframework", "Spring"), ("android.", "Android"), ("javax.swing", "Swing")]),

        new("javascript", "JavaScript", [".js", ".mjs", ".cjs"],
            [@"\bconsole\.log\s*\(", @"\bfunction\s+\w+\s*\(", @"\b(const|let|var)\s+\w+\s*=", @"=>\s*\{"],
            ["document.", "window.", "module.exports", "require("],
            [("react", "React"), ("express", "Express"), ("vue", "Vue"), ("jquery", "jQuery")]),

        new("typescript", "TypeScript", [".ts", ".tsx"],
            [@"\binterface\s+\w+", @"\btype\s+\w+\s*=", @":\s*(string|number|boolean|void)\b", @"\bas\s+const\b"],
            ["export function", "implements ", "readonly ", "enum "],
            [("react", "React"), ("@angular", "Angular"), ("nestjs", "NestJS")]),

        new("go", "Go", [".go"],
            [@"\bpackage\s+\w+", @"\bfunc\s+\w+\s*\(", @"\bfmt\.Print", @":="],
            ["defer ", "go func", "chan ", "err != nil"],
            [("gin-gonic", "Gin"), ("echo", "Echo")]),

        new("rust", "Rust", [".rs"],
            [@"\bfn\s+\w+\s*\(", @"\blet\s+mut\b", @"\bprintln!", @"\bimpl\s+\w+"],
            ["match ", "Option<", "Result<", "cargo"],
            [("tokio", "Tokio"), ("serde", "Serde"), ("actix", "Actix")]),

        new("php", "PHP", [".php"],
            [@"<\?php", @"\$\w+\s*=", @"\becho\s+", @"\bfunction\s+\w+\s*\("],
            ["array(", "->", "namespace "],
            [("laravel", "Laravel"), ("symfony", "Symfony"), ("wordpress", "WordPress")]),

        new("ruby", "Ruby", [".rb"],
            [@"\bdef\s+\w+", @"\bend\b", @"\bputs\s+", @"\brequire\s+['""]"],
            ["attr_accessor", "do |", "nil", "unless "],
            [("rails", "Ruby on Rails"), ("sinatra", "Sinatra")]),

        new("sql", "SQL", [".sql"],
            [@"\bSELECT\b.+\bFROM\b", @"\bINSERT\s+INTO\b", @"\bCREATE\s+TABLE\b", @"\bWHERE\b"],
            ["JOIN ", "GROUP BY", "PRIMARY KEY"],
            []),

        new("html", "HTML", [".html", ".htm"],
            [@"<!DOCTYPE html>", @"<html\b", @"</html>", @"<div\b"],
            ["<head", "<body", "<script"],
            []),

        new("css", "CSS", [".css"],
            [@"[.#][\w-]+\s*\{", @"@media\b", @"display\s*:", @"flex"],
            ["margin:", "padding:", "color:"],
            []),

        new("kotlin", "Kotlin", [".kt", ".kts"],
            [@"\bfun\s+\w+\s*\(", @"\bval\s+\w+", @"\bdata class\b", @"\bcompanion object\b"],
            ["null safety", "suspend ", "when ("],
            [("android", "Android"), ("compose", "Jetpack Compose")]),

        new("swift", "Swift", [".swift"],
            [@"\bfunc\s+\w+\s*\(", @"\blet\s+\w+", @"\bvar\s+\w+", @"\bguard\s+let\b"],
            ["import Foundation", "UIKit", "SwiftUI"],
            [("SwiftUI", "SwiftUI"), ("UIKit", "UIKit")]),

        new("powershell", "PowerShell", [".ps1", ".psm1"],
            [@"^\s*param\s*\(", @"\$\w+\s*=", @"\bWrite-Host\b", @"\bGet-\w+"],
            ["ForEach-Object", "-eq", "Cmdlet"],
            []),

        new("bash", "Bash", [".sh"],
            [@"^#!/bin/(ba)?sh", @"^\s*if\s*\[", @"\becho\s+", @"\$\{?\w+"],
            ["then", "fi", "done"],
            [])
    ];

    public static LanguageGuess Detect(string? source, string? fileName = null)
    {
        if (string.IsNullOrWhiteSpace(source) && string.IsNullOrWhiteSpace(fileName))
        {
            return LanguageGuess.Unknown;
        }

        var text = source ?? "";
        var ext = Path.GetExtension(fileName ?? "").ToLowerInvariant();
        var scores = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);
        var frameworks = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);

        foreach (var profile in Profiles)
        {
            double score = 0;
            if (ext.Length > 0 && profile.Extensions.Contains(ext))
            {
                score += 45;
            }

            foreach (var pattern in profile.Strong)
            {
                if (Regex.IsMatch(text, pattern, RegexOptions.Multiline | RegexOptions.IgnoreCase))
                {
                    score += 14;
                }
            }

            foreach (var keyword in profile.Keywords)
            {
                if (text.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                {
                    score += 4;
                }
            }

            var found = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var (needle, label) in profile.Frameworks)
            {
                if (text.Contains(needle, StringComparison.OrdinalIgnoreCase))
                {
                    found.Add(label);
                    score += 6;
                }
            }

            if (score > 0)
            {
                scores[profile.Id] = score;
                frameworks[profile.Id] = found;
            }
        }

        Disambiguate(text, ext, scores);

        if (scores.Count == 0)
        {
            return LanguageGuess.Unknown;
        }

        var ranked = scores.OrderByDescending(kv => kv.Value).ToList();
        var best = ranked[0];
        var second = ranked.Count > 1 ? ranked[1].Value : 0;
        var confidence = best.Value <= 0
            ? 0
            : Math.Clamp((best.Value - second * 0.35) / Math.Max(best.Value, 40), 0.15, 0.99);

        var winner = Profiles.First(p => p.Id == best.Key);
        frameworks.TryGetValue(best.Key, out var fw);

        return new LanguageGuess
        {
            Id = winner.Id,
            Name = winner.Name,
            Confidence = Math.Round(confidence, 2),
            Frameworks = fw is { Count: > 0 } ? fw.OrderBy(x => x).ToArray() : []
        };
    }

    private static void Disambiguate(string text, string ext, Dictionary<string, double> scores)
    {
        if (ext is ".h" && scores.ContainsKey("cpp") && scores.ContainsKey("c"))
        {
            scores["c"] += text.Contains("class ", StringComparison.Ordinal) ? -10 : 8;
        }

        if (scores.ContainsKey("typescript") && scores.ContainsKey("javascript"))
        {
            if (Regex.IsMatch(text, @"\b(interface|type)\s+\w+|:\s*(string|number|boolean)\b"))
            {
                scores["typescript"] += 20;
                scores["javascript"] -= 8;
            }
        }

        if (scores.ContainsKey("python") && scores.ContainsKey("ruby"))
        {
            if (text.Contains("elif ") || text.Contains("self."))
            {
                scores["python"] += 12;
            }

            if (Regex.IsMatch(text, @"^\s*end\s*$", RegexOptions.Multiline))
            {
                scores["ruby"] += 12;
            }
        }
    }
}
