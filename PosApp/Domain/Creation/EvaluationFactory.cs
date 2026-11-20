using PosApp.Domain.Models;

namespace PosApp.Domain.Creation;

public sealed class EvaluationFactory(TimeProvider timeProvider) : IEvaluationFactory
{
    public Evaluation Create(Rubric rubric, CriterionId criterionId, decimal rawScore, string? comment = null)
    {
        ArgumentNullException.ThrowIfNull(rubric);

        var criterion = rubric.FindCriterion(criterionId)
            ?? throw new InvalidOperationException(
                $"Критерий {criterionId} не найден в рубрикаторе курса «{rubric.CourseName}».");

        var score = new Score(rawScore, criterion.MaxScore);
        return new Evaluation(criterion.Id, score, timeProvider.GetUtcNow(), comment);
    }
}