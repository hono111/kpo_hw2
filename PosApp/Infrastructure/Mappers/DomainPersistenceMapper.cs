using PosApp.Domain.Models;
using PosApp.Infrastructure.Dtos;
using Riok.Mapperly.Abstractions;

namespace PosApp.Infrastructure.Mappers;

[Mapper]
internal static partial class DomainPersistenceMapper
{
    internal static partial StudentPersistenceDto ToDto(this Student student);

    internal static Student ToDomain(this StudentPersistenceDto dto) =>
        new(StudentId.From(dto.Id), dto.FullName, dto.Group);

    internal static Rubric ToDomain(this RubricPersistenceDto dto)
    {
        var criteria = dto.Criteria.Select(c =>
            new Criterion(
                CriterionId.From(c.Id),
                c.Title,
                new MaxScore(c.MaxScore),
                new Weight(c.Weight)));

        return Rubric.Restore(dto.CourseName, criteria);
    }

    internal static RubricPersistenceDto ToDto(this Rubric rubric) =>
        new(
            rubric.CourseName,
            rubric.Criteria
                .Select(c => new CriterionPersistenceDto(c.Id.Value, c.Title, c.MaxScore.Value, c.Weight.Value))
                .ToList());

    internal static StudentGradebook ToDomain(this GradebookPersistenceDto dto)
    {
        var evaluations = dto.Evaluations.Select(e =>
            Evaluation.Restore(
                CriterionId.From(e.CriterionId),
                e.Score,
                e.EvaluatedAt,
                e.Comment));

        return StudentGradebook.Restore(StudentId.From(dto.StudentId), evaluations);
    }

    internal static GradebookPersistenceDto ToDto(this StudentGradebook gradebook) =>
        new(
            gradebook.StudentId.Value,
            gradebook.Evaluations.Select(ToDto).ToList());

    private static partial EvaluationPersistenceDto ToDto(this Evaluation evaluation);

    private static Guid MapStudentId(StudentId id) => id.Value;
    private static Guid MapCriterionId(CriterionId id) => id.Value;
    private static decimal MapScore(Score score) => score.Value;
}