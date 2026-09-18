using System.Text.Json.Serialization;

namespace CodeBrief.Contracts;

public sealed class AnalysisRequest
{
    public required string Source { get; init; }
    public string? FileName { get; init; }
    public LanguageGuess? ClientGuess { get; init; }
}

public sealed class LanguageGuess
{
    public static LanguageGuess Unknown { get; } = new()
    {
        Id = "unknown",
        Name = "Okänt",
        Confidence = 0
    };

    [JsonPropertyName("id")]
    public required string Id { get; init; }

    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonPropertyName("frameworks")]
    public IReadOnlyList<string> Frameworks { get; init; } = [];

    [JsonPropertyName("confidence")]
    public double Confidence { get; init; }
}

public sealed class AnalysisReport
{
    [JsonPropertyName("language")]
    public LanguageInfo Language { get; set; } = new();

    [JsonPropertyName("section_1_overview")]
    public OverviewSection Overview { get; set; } = new();

    [JsonPropertyName("section_2_pseudocode")]
    public PseudocodeSection Pseudocode { get; set; } = new();

    [JsonPropertyName("section_3_flow")]
    public FlowSection Flow { get; set; } = new();

    [JsonPropertyName("section_4_issues")]
    public IssuesSection Issues { get; set; } = new();

    [JsonPropertyName("section_5_improvements")]
    public ImprovementsSection Improvements { get; set; } = new();

    [JsonPropertyName("section_6_fix")]
    public FixSection Fix { get; set; } = new();

    [JsonPropertyName("generated_at")]
    public DateTimeOffset GeneratedAt { get; set; } = DateTimeOffset.UtcNow;

    [JsonPropertyName("analyzer")]
    public string Analyzer { get; set; } = "local";
}

public sealed class LanguageInfo
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = "unknown";

    [JsonPropertyName("name")]
    public string Name { get; set; } = "Okänt";

    [JsonPropertyName("frameworks")]
    public IReadOnlyList<string> Frameworks { get; set; } = [];

    [JsonPropertyName("complexity")]
    public string Complexity { get; set; } = "low";
}

public sealed class OverviewSection
{
    [JsonPropertyName("summary")]
    public string Summary { get; set; } = "";
}

public sealed class PseudocodeSection
{
    [JsonPropertyName("lines")]
    public IReadOnlyList<string> Lines { get; init; } = [];
}

public sealed class FlowSection
{
    [JsonPropertyName("inputs")]
    public IReadOnlyList<string> Inputs { get; init; } = [];

    [JsonPropertyName("steps")]
    public IReadOnlyList<string> Steps { get; init; } = [];

    [JsonPropertyName("outputs")]
    public IReadOnlyList<string> Outputs { get; init; } = [];
}

public sealed class IssuesSection
{
    [JsonPropertyName("is_correct")]
    public bool IsCorrect { get; set; }

    [JsonPropertyName("bugs")]
    public IReadOnlyList<Finding> Bugs { get; set; } = [];

    [JsonPropertyName("edge_cases")]
    public IReadOnlyList<Finding> EdgeCases { get; set; } = [];

    [JsonPropertyName("security")]
    public IReadOnlyList<Finding> Security { get; set; } = [];
}

public sealed class Finding
{
    [JsonPropertyName("severity")]
    public string Severity { get; init; } = "medium";

    [JsonPropertyName("title")]
    public string Title { get; init; } = "";

    [JsonPropertyName("detail")]
    public string Detail { get; init; } = "";
}

public sealed class ImprovementsSection
{
    [JsonPropertyName("performance")]
    public IReadOnlyList<string> Performance { get; init; } = [];

    [JsonPropertyName("maintainability")]
    public IReadOnlyList<string> Maintainability { get; init; } = [];
}

public sealed class FixSection
{
    [JsonPropertyName("needed")]
    public bool Needed { get; init; }

    [JsonPropertyName("explanation")]
    public string Explanation { get; init; } = "";

    [JsonPropertyName("corrected_code")]
    public string? CorrectedCode { get; init; }
}
