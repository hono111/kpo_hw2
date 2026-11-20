using PosApp.Domain.Models;
using PosApp.Domain.Strategies;

namespace PosApp.Domain.Services;

public sealed class FinalGradeCalculationService : IFinalGradeCalculator
{
    public FinalGrade CalculateFinalGrade(StudentGradebook gradebook, Rubric rubric, IGradingStrategy strategy)
    {
        ArgumentNullException.ThrowIfNull(gradebook);
        ArgumentNullException.ThrowIfNull(rubric);
        ArgumentNullException.ThrowIfNull(strategy);

        var rawResult = strategy.Calculate(gradebook, rubric);
        var evaluatedCount = rubric.Criteria.Count(c => gradebook.GetEvaluation(c.Id) is not null);

        return new FinalGrade(
            StudentId: gradebook.StudentId,
            Value: rawResult,
            StrategyName: strategy.Name,
            EvaluatedCriteriaCount: evaluatedCount,
            TotalCriteriaCount: rubric.Criteria.Count);
    }
}

public sealed class RoundingGradeCalculatorDecorator<TCalculator>(
    TCalculator inner,
    int decimals = 2,
    MidpointRounding roundingMode = MidpointRounding.AwayFromZero) : IFinalGradeCalculator
    where TCalculator : IFinalGradeCalculator
{
    public FinalGrade CalculateFinalGrade(StudentGradebook gradebook, Rubric rubric, IGradingStrategy strategy)
    {
        var baseResult = inner.CalculateFinalGrade(gradebook, rubric, strategy);
        var rounded = Math.Round(baseResult.Value, decimals, roundingMode);

        return baseResult with
        {
            Value = rounded,
            StrategyName = $"{baseResult.StrategyName} [округление: {decimals} зн.]"
        };
    }
}

public static class GradeCalculatorDecoratorExtensions
{
    public static IFinalGradeCalculator WithRounding<TCalculator>(
        this TCalculator calculator,
        int decimals = 2,
        MidpointRounding roundingMode = MidpointRounding.AwayFromZero)
        where TCalculator : IFinalGradeCalculator =>
        new RoundingGradeCalculatorDecorator<TCalculator>(calculator, decimals, roundingMode);
}