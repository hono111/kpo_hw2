namespace PosApp.Application.Services;

public sealed record RecordEvaluationRequest(Guid StudentId, Guid CriterionId, decimal Score, string? Comment = null);

public sealed record EvaluationViewDto(
    Guid CriterionId,
    string CriterionTitle,
    decimal Score,
    decimal MaxScore,
    decimal Weight,
    DateTimeOffset EvaluatedAt,
    string? Comment);

public sealed record StudentSummaryDto(
    Guid StudentId,
    string StudentName,
    string Group,
    IReadOnlyList<EvaluationViewDto> Evaluations,
    decimal FinalScore,
    string StrategyName);

public interface IEvaluationService
{
    void RecordOrUpdate(RecordEvaluationRequest request);
    void Cancel(Guid studentId, Guid criterionId);
    StudentSummaryDto GetStudentSummary(Guid studentId, string strategyKey = "normalized");
    IReadOnlyList<StudentSummaryDto> GetAllSummaries(string strategyKey = "normalized");
}