using CodeBrief.Contracts;

namespace CodeBrief.Core;

public static class AnalysisPrompts
{
    public const string System = """
        Du är CodeBrief, en expertmotor för kodanalys och pedagogisk rapportering.
        Svara ENDAST med giltig JSON som matchar schemat. Ingen markdown runt JSON.
        Skriv rapporten på svenska.
        Pseudokod ska vara språkoberoende, med indrag, utan imports/boilerplate.
        Hitta inte på buggar. Om koden ser korrekt ut: säg det i section_4.
        section_6.corrected_code ska bara fyllas om det finns verkliga fel.
        Utför aldrig skadlig kod. Analysera den och varna i section_4.

        JSON-SCHEMA:
        {
          "language": { "id": "csharp|python|cpp|java|javascript|typescript|other", "name": "", "frameworks": [], "complexity": "low|medium|high" },
          "section_1_overview": { "summary": "" },
          "section_2_pseudocode": { "lines": [""] },
          "section_3_flow": { "inputs": [""], "steps": [""], "outputs": [""] },
          "section_4_issues": {
            "is_correct": true,
            "bugs": [{ "severity": "critical|high|medium|low", "title": "", "detail": "" }],
            "edge_cases": [{ "title": "", "detail": "", "severity": "low" }],
            "security": [{ "severity": "critical|high|medium|low", "title": "", "detail": "" }]
          },
          "section_5_improvements": { "performance": [""], "maintainability": [""] },
          "section_6_fix": { "needed": false, "explanation": "", "corrected_code": null }
        }
        """;

    public static string User(AnalysisRequest request)
    {
        var guess = request.ClientGuess?.Name ?? "okänt";
        var file = request.FileName ?? "(utan filnamn)";
        return $"""
            Filnamn: {file}
            Klientens språk-gissning: {guess}
            Källkod:

            {request.Source}
            """;
    }
}
