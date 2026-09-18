using System.Text.RegularExpressions;
using CodeBrief.Contracts;

namespace CodeBrief.Core;

/// <summary>
/// Local, offline analyzer used until the cloud AI proxy is wired in.
/// Finds high-signal issues and produces a readable 6-section report.
/// </summary>
public sealed class HeuristicCodeAnalyzer : ICodeAnalyzer
{
    public string Name => "Lokal heuristik (v0.1)";

    public Task<AnalysisReport> AnalyzeAsync(AnalysisRequest request, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var source = request.Source ?? "";
        var guess = request.ClientGuess ?? LanguageDetector.Detect(source, request.FileName);
        var lines = SplitLines(source);
        var complexity = EstimateComplexity(lines);
        var findings = FindIssues(source, guess.Id);
        var bugs = findings.Where(f => f.Kind == "bug").Select(ToFinding).ToList();
        var edge = findings.Where(f => f.Kind == "edge").Select(ToFinding).ToList();
        var security = findings.Where(f => f.Kind == "security").Select(ToFinding).ToList();
        var isCorrect = bugs.Count == 0 && security.All(s => s.Severity is not ("critical" or "high"));
        var fix = BuildFix(source, guess.Id, findings);

        var report = new AnalysisReport
        {
            Analyzer = Name,
            GeneratedAt = DateTimeOffset.Now,
            Language = new LanguageInfo
            {
                Id = guess.Id,
                Name = guess.Name,
                Frameworks = guess.Frameworks,
                Complexity = complexity
            },
            Overview = new OverviewSection
            {
                Summary = BuildSummary(source, guess, complexity, lines.Length, findings.Count)
            },
            Pseudocode = new PseudocodeSection
            {
                Lines = ToPseudocode(source, guess.Id)
            },
            Flow = new FlowSection
            {
                Inputs = ExtractInputs(source, guess.Id, request.FileName),
                Steps = ExtractSteps(source, guess.Id, lines),
                Outputs = ExtractOutputs(source, guess.Id)
            },
            Issues = new IssuesSection
            {
                IsCorrect = isCorrect,
                Bugs = bugs,
                EdgeCases = edge,
                Security = security
            },
            Improvements = new ImprovementsSection
            {
                Performance = BuildPerformanceTips(source, guess.Id),
                Maintainability = BuildMaintainabilityTips(source, guess.Id, lines.Length)
            },
            Fix = fix
        };

        return Task.FromResult(report);
    }

    private static string[] SplitLines(string source) =>
        source.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');

    private static string EstimateComplexity(string[] lines)
    {
        var codeLines = lines.Count(l => !string.IsNullOrWhiteSpace(l));
        var decisions = Regex.Matches(
            string.Join('\n', lines),
            @"\b(if|elif|else if|else|switch|case|for|while|catch|try|&&|\|\||\?)\b",
            RegexOptions.IgnoreCase).Count;

        if (codeLines > 400 || decisions > 60)
        {
            return "high";
        }

        if (codeLines > 80 || decisions > 15)
        {
            return "medium";
        }

        return "low";
    }

    private static string BuildSummary(string source, LanguageGuess guess, string complexity, int lineCount, int issueCount)
    {
        var lang = guess.Name == "Okänt" ? "ett okänt språk" : guess.Name;
        var fw = guess.Frameworks.Count > 0
            ? $" Ramverk som syns i koden: {string.Join(", ", guess.Frameworks)}."
            : "";
        var issueText = issueCount == 0
            ? "Inga uppenbara buggar eller säkerhetshål hittades med den lokala analysen."
            : $"Den lokala analysen flaggade {issueCount} sak{(issueCount == 1 ? "" : "er")} att granska.";
        var complexitySv = complexity switch
        {
            "high" => "hög",
            "medium" => "medel",
            _ => "låg"
        };

        return $"Det här ser ut som {lang}-kod på ungefär {lineCount} rader, med {complexitySv} komplexitet.{fw} " +
               $"{issueText} En djupare AI-analys kopplas på i nästa version — den här rapporten bygger på mönster, inte en språkmodell.";
    }

    private static IReadOnlyList<string> ToPseudocode(string source, string languageId)
    {
        var raw = SplitLines(source);
        var indent = 0;
        var result = new List<string>();

        foreach (var original in raw)
        {
            var trimmed = original.Trim();
            if (trimmed.Length == 0)
            {
                continue;
            }

            if (trimmed.StartsWith('}') || trimmed is "end" or "fi" or "done")
            {
                indent = Math.Max(0, indent - 1);
            }

            var line = TranslateLine(trimmed.TrimEnd('{', '}', ';'), languageId);
            if (line.Length > 0)
            {
                result.Add(new string(' ', indent * 4) + line);
            }

            if (trimmed.EndsWith('{') || Regex.IsMatch(trimmed, @"^\s*(if|for|while|def|fun|func|class|switch|try|else)\b") && languageId is "python" or "ruby" or "bash")
            {
                indent++;
            }
        }

        if (result.Count == 0)
        {
            return ["(Ingen körbar logik att översätta.)"];
        }

        if (result.Count > 90)
        {
            var kept = result.Take(90).ToList();
            kept.Add($"... ytterligare {result.Count - 90} rader dolda.");
            return kept;
        }

        return result;
    }

    private static string TranslateLine(string line, string languageId)
    {
        var s = Regex.Replace(line, @"^\s*//.*$", "");
        s = Regex.Replace(s, @"^\s*#(?!!).*", "");
        s = s.Trim();
        if (s.Length == 0 || s is "{" or "}")
        {
            return "";
        }

        s = Regex.Replace(s, @"\bpublic static void Main\s*\([^)]*\)", "STARTA PROGRAMMET");
        s = Regex.Replace(s, @"\bint main\s*\([^)]*\)", "STARTA PROGRAMMET");
        s = Regex.Replace(s, @"\bif\s*\((.+)\)", "OM $1");
        s = Regex.Replace(s, @"^\s*if\s+(.+):$", "OM $1");
        s = Regex.Replace(s, @"\belif\s+(.+):", "ANNARS OM $1");
        s = Regex.Replace(s, @"\belse if\s*\((.+)\)", "ANNARS OM $1");
        s = Regex.Replace(s, @"\belse\b", "ANNARS");
        s = Regex.Replace(s, @"\bfor\s*\((.+)\)", "UPPREPA FÖR $1");
        s = Regex.Replace(s, @"\bforeach\s*\((.+)\)", "FÖR VARJE $1");
        s = Regex.Replace(s, @"\bfor\s+(\w+)\s+in\s+(.+):", "FÖR VARJE $1 I $2");
        s = Regex.Replace(s, @"\bwhile\s*\((.+)\)", "SÅ LÄNGE $1");
        s = Regex.Replace(s, @"\bwhile\s+(.+):", "SÅ LÄNGE $1");
        s = Regex.Replace(s, @"\breturn\b", "RETURNERA");
        s = Regex.Replace(s, @"\b(print|printf|puts|echo|Console\.WriteLine|System\.out\.println|fmt\.Println)\s*\(?(.*)\)?", "SKRIV UT $2");
        s = Regex.Replace(s, @"\b(def|function|fun|func|fn)\s+", "FUNKTION ");
        s = Regex.Replace(s, @"\bclass\s+", "KLASS ");
        s = Regex.Replace(s, @"\bcatch\s*\((.+)\)", "FÅNGA FEL ($1)");
        s = Regex.Replace(s, @"\btry\b", "FÖRSÖK");
        s = Regex.Replace(s, @"\busing\s+", "ANVÄND ");
        s = Regex.Replace(s, @"\bimport\s+", "IMPORTERA ");
        s = Regex.Replace(s, @"\bnamespace\s+", "NAMNRYMD ");
        _ = languageId;
        return s;
    }

    private static IReadOnlyList<string> ExtractInputs(string source, string languageId, string? fileName)
    {
        var inputs = new List<string>();
        if (!string.IsNullOrWhiteSpace(fileName))
        {
            inputs.Add($"Fil: {fileName}");
        }

        foreach (Match m in Regex.Matches(source, @"\b(args|argv|sys\.argv|string\[\]\s+args)\b"))
        {
            inputs.Add("Kommandoradsargument");
            break;
        }

        if (Regex.IsMatch(source, @"\b(Console\.Read|input\s*\(|scanf|readLine|prompt\s*\()"))
        {
            inputs.Add("Användarinmatning från tangentbord/konsol");
        }

        foreach (Match m in Regex.Matches(source, @"(?:def|function|fun|func|fn|void|int|string|Task|public)\s+\w+\s*\(([^)]*)\)"))
        {
            var parameters = m.Groups[1].Value.Trim();
            if (parameters.Length > 0 && parameters is not "void")
            {
                inputs.Add($"Parametrar: {parameters}");
            }
        }

        if (inputs.Count == 0)
        {
            inputs.Add("Ingen yttre indata syns — koden verkar använda hårdkodade värden.");
        }

        return inputs.Distinct().Take(8).ToArray();
    }

    private static IReadOnlyList<string> ExtractSteps(string source, string languageId, string[] lines)
    {
        var steps = new List<string>
        {
            "Start: koden läses in och språket identifieras.",
            $"Bearbetning: {Math.Max(1, lines.Count(l => !string.IsNullOrWhiteSpace(l)))} rader körs uppifrån och ned."
        };

        if (Regex.IsMatch(source, @"\b(Main|main|if __name__)\b"))
        {
            steps.Add("En startpunkt (main) hittades — programmet börjar där.");
        }

        if (Regex.IsMatch(source, @"\b(for|foreach|while|map|forEach)\b"))
        {
            steps.Add("En eller flera loopar upprepar arbete över data.");
        }

        if (Regex.IsMatch(source, @"\b(if|switch|match|elif)\b"))
        {
            steps.Add("Villkor styr vilken gren som körs.");
        }

        if (Regex.IsMatch(source, @"\b(try|catch|except|Result<|Option<)\b"))
        {
            steps.Add("Felhantering finns — exekveringen kan hoppa till en felgren.");
        }

        steps.Add("Slut: funktioner returnerar, skriver ut eller sparar resultat.");
        _ = languageId;
        return steps;
    }

    private static IReadOnlyList<string> ExtractOutputs(string source, string languageId)
    {
        var outputs = new List<string>();
        if (Regex.IsMatch(source, @"\b(Console\.Write|print\s*\(|printf|System\.out|fmt\.Print|echo |puts )\b"))
        {
            outputs.Add("Utskrift till konsolen");
        }

        if (Regex.IsMatch(source, @"\b(File\.(Write|Save)|writeFile|open\([^)]*[""']w|to_csv|SaveAs)\b"))
        {
            outputs.Add("Data skrivs till fil");
        }

        if (Regex.IsMatch(source, @"\breturn\b"))
        {
            outputs.Add("Returvärde från funktion");
        }

        if (outputs.Count == 0)
        {
            outputs.Add("Ingen tydlig utdata hittades (inga print/return/filskrivningar).");
        }

        _ = languageId;
        return outputs;
    }

    private sealed record Hit(string Kind, string Severity, string Title, string Detail);

    private static List<Hit> FindIssues(string source, string languageId)
    {
        var hits = new List<Hit>();

        if (Regex.IsMatch(source, @"/\s*0\b|/\s*n\b"))
        {
            hits.Add(new("bug", "high", "Möjlig division med noll",
                "Koden delar med en variabel eller bokstavlig 0. Om nämnaren blir noll kraschar programmet."));
        }

        if (Regex.IsMatch(source, @"catch\s*\([^)]*\)\s*\{\s*\}", RegexOptions.Singleline))
        {
            hits.Add(new("bug", "medium", "Tom catch-sats",
                "Ett undantag fångas men ignoreras. Fel sväljs tyst och blir svåra att felsöka."));
        }

        if (Regex.IsMatch(source, @"except\s*(Exception)?\s*:\s*(pass|\.\.\.)"))
        {
            hits.Add(new("bug", "medium", "Undantag tystas",
                "Python-koden fångar fel och gör pass. Användaren får inget felmeddelande."));
        }

        if (Regex.IsMatch(source, @"\.Result\b|\.Wait\s*\(\s*\)"))
        {
            hits.Add(new("bug", "high", "Sync-over-async",
                ".Result eller .Wait() på en Task kan låsa tråden (deadlock) i UI-appar."));
        }

        if (Regex.IsMatch(source, @"\b(password|passwd|pwd|api[_-]?key|secret|token)\s*=\s*[""'][^""']+[""']", RegexOptions.IgnoreCase))
        {
            hits.Add(new("security", "critical", "Hårdkodad hemlighet",
                "Ett lösenord, en nyckel eller en token ligger i källkoden. Flytta till miljövariabel eller hemlighetslagring."));
        }

        if (Regex.IsMatch(source, @"\beval\s*\(|\bexec\s*\(|pickle\.loads|innerHTML\s*="))
        {
            hits.Add(new("security", "high", "Farlig dynamisk körning",
                "eval/exec/innerHTML/pickle kan köra otillförlitlig data som kod. Validera eller undvik helt."));
        }

        if (Regex.IsMatch(source, @"(SELECT|INSERT|DELETE|UPDATE).*\+|\$"".*SELECT|f[""'].*SELECT", RegexOptions.IgnoreCase | RegexOptions.Singleline))
        {
            hits.Add(new("security", "critical", "Möjlig SQL-injektion",
                "SQL byggs med strängkonkatenering. Använd parameteriserade frågor."));
        }

        if (Regex.IsMatch(source, @"shell\s*=\s*True|Process\.Start\s*\(\s*[\$""]"))
        {
            hits.Add(new("security", "high", "Kommandoinjektion",
                "Yttre data kan nå en processstart. Undvik shell=True och interpolera inte kommandorader."));
        }

        if (Regex.IsMatch(source, @"\bMD5\b|md5\(|hashlib\.md5"))
        {
            hits.Add(new("security", "medium", "Svag hash (MD5)",
                "MD5 är bruten för lösenord. Använd en modern KDF (t.ex. Argon2, bcrypt eller PBKDF2)."));
        }

        if (languageId is "csharp" or "java" or "javascript" or "typescript" &&
            Regex.IsMatch(source, @"\[\s*\d+\s*\]") && !Regex.IsMatch(source, @"Length|Count|length"))
        {
            hits.Add(new("edge", "medium", "Indexering utan längdkontroll",
                "Arrayer indexeras utan synlig kontroll av längd. Tomma samlingar ger krasch eller fel element."));
        }

        if (Regex.IsMatch(source, @"\bnull\b|\bundefined\b|\bNone\b") &&
            !Regex.IsMatch(source, @"\bis null|\?\.|!= null|is not None|??"))
        {
            hits.Add(new("edge", "low", "Null/None används utan synligt skydd",
                "Värden kan vara tomma. Överväg null-kontroller eller null-safe operatorer."));
        }

        if (Regex.IsMatch(source, @"TODO|FIXME|HACK", RegexOptions.IgnoreCase))
        {
            hits.Add(new("edge", "low", "Oavslutad kod",
                "Det finns TODO/FIXME-markeringar. Funktionen är förmodligen inte färdig."));
        }

        return hits;
    }

    private static Finding ToFinding(Hit hit) => new()
    {
        Severity = hit.Severity,
        Title = hit.Title,
        Detail = hit.Detail
    };

    private static IReadOnlyList<string> BuildPerformanceTips(string source, string languageId)
    {
        var tips = new List<string>();
        if (Regex.IsMatch(source, @"for\s*\(.*\)\s*\{[^}]*for\s*\(", RegexOptions.Singleline))
        {
            tips.Add("Nästlade loopar hittades. Kontrollera att de inte gör N×M onödigt arbete, särskilt mot databas eller disk.");
        }

        if (Regex.IsMatch(source, @"\+\s*[""']|\.Add\s*\(.*in\s+.*for", RegexOptions.IgnoreCase))
        {
            tips.Add("Strängar byggs troligen med + i loop. Använd StringBuilder / join för stora mängder text.");
        }

        if (tips.Count == 0)
        {
            tips.Add("Inga tunga mönster stack ut. Mät innan du optimerar.");
        }

        _ = languageId;
        return tips;
    }

    private static IReadOnlyList<string> BuildMaintainabilityTips(string source, string languageId, int lineCount)
    {
        var tips = new List<string>
        {
            "Döp funktioner efter vad de gör, inte hur de gör det.",
            "Bryt ut långa block till små funktioner som går att testa var för sig."
        };

        if (lineCount > 200)
        {
            tips.Insert(0, "Filen är lång. Dela upp i flera moduler med ett ansvar vardera.");
        }

        if (!Regex.IsMatch(source, @"\b(test|spec|assert|Should|Fact|It\()", RegexOptions.IgnoreCase))
        {
            tips.Add("Inga tester syns i utdraget. Lägg till minst ett test kring den farligaste grenen (t.ex. noll, tom lista, ogiltig input).");
        }

        _ = languageId;
        return tips.Take(5).ToArray();
    }

    private static FixSection BuildFix(string source, string languageId, List<Hit> findings)
    {
        var needsFix = findings.Any(f => f.Kind is "bug" or "security" && f.Severity is "critical" or "high");
        if (!needsFix)
        {
            return new FixSection
            {
                Needed = false,
                Explanation = "Inga kritiska fel som den lokala motorn kan rätta automatiskt.",
                CorrectedCode = null
            };
        }

        var fixedCode = source;
        if (languageId == "csharp" && Regex.IsMatch(source, @"/\s*n\b"))
        {
            fixedCode = Regex.Replace(
                fixedCode,
                @"100\s*/\s*n",
                "n == 0 ? 0 : 100 / n");
        }

        if (Regex.IsMatch(fixedCode, @"\b(password|api[_-]?key|secret)\s*=\s*[""'][^""']+[""']", RegexOptions.IgnoreCase))
        {
            fixedCode = Regex.Replace(
                fixedCode,
                @"(password|api[_-]?key|secret)\s*=\s*[""'][^""']+[""']",
                "$1 = Environment.GetEnvironmentVariable(\"APP_SECRET\")",
                RegexOptions.IgnoreCase);
        }

        return new FixSection
        {
            Needed = true,
            Explanation = "Förslaget är en mekanisk rättning av de mest uppenbara felen. Läs igenom innan du använder det.",
            CorrectedCode = fixedCode == source ? null : fixedCode
        };
    }
}
