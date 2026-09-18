using CodeBrief.Contracts;

namespace CodeBrief.Core;

public interface ICodeAnalyzer
{
    string Name { get; }

    Task<AnalysisReport> AnalyzeAsync(AnalysisRequest request, CancellationToken cancellationToken = default);
}
