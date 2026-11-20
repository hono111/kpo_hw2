namespace PosApp.Infrastructure.Dtos;

internal sealed record StudentPersistenceDto(Guid Id, string FullName, string Group);

internal sealed record CriterionPersistenceDto(Guid Id, string Title, decimal MaxScore, decimal Weight);

internal sealed record RubricPersistenceDto(string CourseName, List<CriterionPersistenceDto> Criteria);

internal sealed record EvaluationPersistenceDto(
    Guid CriterionId,
    decimal Score,
    DateTimeOffset EvaluatedAt,
    string? Comment);

internal sealed record GradebookPersistenceDto(
    Guid StudentId,
    List<EvaluationPersistenceDto> Evaluations);

internal sealed class PosDatabaseDto
{
    public Dictionary<Guid, StudentPersistenceDto> Students { get; set; } = [];
    public RubricPersistenceDto? Rubric { get; set; }
    public Dictionary<Guid, GradebookPersistenceDto> Gradebooks { get; set; } = [];
}