using PosApp.Domain.Models;

namespace PosApp.Domain.Creation;

public interface IEvaluationFactory
{
    Evaluation Create(Rubric rubric, CriterionId criterionId, decimal rawScore, string? comment = null);
}