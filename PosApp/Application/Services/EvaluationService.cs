using PosApp.Domain.Creation;
using PosApp.Domain.Models;
using PosApp.Domain.Repositories;
using PosApp.Domain.Services;
using PosApp.Domain.Strategies;

namespace PosApp.Application.Services;

internal sealed class EvaluationService(
    IStudentRepository studentRepository,
    IRubricRepository rubricRepository,
    IGradebookRepository gradebookRepository,
    IEvaluationFactory evaluationFactory,
    IFinalGradeCalculator gradeCalculator,
    IReadOnlyDictionary<string, IGradingStrategy> strategies) : IEvaluationService
{
    public void RecordOrUpdate(RecordEvaluationRequest request)
    {
        var studentId = StudentId.From(request.StudentId);
        if (studentRepository.GetById(studentId) is null)
            throw new InvalidOperationException($"Студент с ID {request.StudentId} не найден.");

        var criterionId = CriterionId.From(request.CriterionId);
        var rubric = rubricRepository.Get();
        var gradebook = gradebookRepository.GetOrCreate(studentId);

        var evaluation = evaluationFactory.Create(rubric, criterionId, request.Score, request.Comment);

        gradebook.RecordEvaluation(evaluation, rubric);
        gradebookRepository.Save(gradebook);
    }

    public void Cancel(Guid studentId, Guid criterionId)
    {
        var sId = StudentId.From(studentId);
        if (studentRepository.GetById(sId) is null)
            throw new InvalidOperationException($"Студент с ID {studentId} не найден.");

        var gradebook = gradebookRepository.GetOrCreate(sId);
        gradebook.CancelEvaluation(CriterionId.From(criterionId));
        gradebookRepository.Save(gradebook);
    }

    public StudentSummaryDto GetStudentSummary(Guid studentId, string strategyKey = "normalized")
    {
        var sId = StudentId.From(studentId);
        var student = studentRepository.GetById(sId)
            ?? throw new InvalidOperationException($"Студент с ID {studentId} не найден.");

        var rubric = rubricRepository.Get();
        var gradebook = gradebookRepository.GetOrCreate(sId);
        var strategy = ResolveStrategy(strategyKey);

        var finalGrade = gradeCalculator.CalculateFinalGrade(gradebook, rubric, strategy);

        var views = rubric.Criteria
            .Select(c =>
            {
                var ev = gradebook.GetEvaluation(c.Id);
                return ev is null
                    ? null
                    : new EvaluationViewDto(
                        c.Id.Value,
                        c.Title,
                        ev.Score.Value,
                        c.MaxScore.Value,
                        c.Weight.Value,
                        ev.EvaluatedAt,
                        ev.Comment);
            })
            .Where(x => x is not null)
            .Cast<EvaluationViewDto>()
            .ToList();

        return new StudentSummaryDto(
            student.Id.Value,
            student.FullName,
            student.Group,
            views,
            finalGrade.Value,
            finalGrade.StrategyName);
    }

    public IReadOnlyList<StudentSummaryDto> GetAllSummaries(string strategyKey = "normalized") =>
        studentRepository.List()
            .Select(s => GetStudentSummary(s.Id.Value, strategyKey))
            .ToList();

    private IGradingStrategy ResolveStrategy(string key) =>
        strategies.TryGetValue(key, out var strategy)
            ? strategy
            : throw new ArgumentException($"Неизвестная стратегия расчёта итога: «{key}». Доступны: {string.Join(", ", strategies.Keys)}.");
}