using PosApp.Domain.Models;
using PosApp.Domain.Repositories;

namespace PosApp.Application.Services;

internal sealed class RubricService(
    IRubricRepository rubricRepository,
    IGradebookRepository gradebookRepository) : IRubricService
{
    public RubricDto Get()
    {
        var rubric = rubricRepository.Get();
        var items = rubric.Criteria
            .Select(c => new CriterionDto(c.Id.Value, c.Title, c.MaxScore.Value, c.Weight.Value))
            .ToList();

        return new RubricDto(rubric.CourseName, items);
    }

    public CriterionDto AddCriterion(AddCriterionRequest request)
    {
        var rubric = rubricRepository.Get();
        var criterion = rubric.AddCriterion(
            request.Title,
            new MaxScore(request.MaxScore),
            new Weight(request.Weight));

        rubricRepository.Save(rubric);
        return new CriterionDto(criterion.Id.Value, criterion.Title, criterion.MaxScore.Value, criterion.Weight.Value);
    }

    public void UpdateCriterion(UpdateCriterionRequest request)
    {
        var id = CriterionId.From(request.CriterionId);
        var rubric = rubricRepository.Get();
        var newMax = new MaxScore(request.MaxScore);

        foreach (var gradebook in gradebookRepository.List())
        {
            var existing = gradebook.GetEvaluation(id);
            if (existing is not null && existing.Score.Value > newMax.Value)
                throw new InvalidOperationException(
                    $"Нельзя уменьшить максимум до {newMax.Value}: у студента {gradebook.StudentId} уже стоит балл {existing.Score.Value}.");
        }

        rubric.UpdateCriterion(id, request.Title, newMax, new Weight(request.Weight));
        rubricRepository.Save(rubric);
    }

    public void RemoveCriterion(Guid criterionId)
    {
        var id = CriterionId.From(criterionId);
        var rubric = rubricRepository.Get();

        if (gradebookRepository.List().Any(g => g.GetEvaluation(id) is not null))
            throw new InvalidOperationException("Нельзя удалить критерий, по которому студентам уже выставлены оценки.");

        rubric.RemoveCriterion(id);
        rubricRepository.Save(rubric);
    }

    public void ReplaceRubric(Rubric rubric)
    {
        ArgumentNullException.ThrowIfNull(rubric);
        rubricRepository.Save(rubric);
    }
}