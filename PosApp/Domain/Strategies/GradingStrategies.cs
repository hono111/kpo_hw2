using PosApp.Domain.Models;

namespace PosApp.Domain.Strategies;

public sealed class WeightedNormalizedStrategy(decimal targetScale = 10m) : IGradingStrategy
{
    public string Name => $"Взвешенное нормированное (шкала 0–{targetScale:0.##})";

    public decimal Calculate(StudentGradebook gradebook, Rubric rubric)
    {
        if (rubric.Criteria.Count == 0)
            return 0m;

        decimal weightedRatioSum = 0m;
        decimal totalWeight = 0m;

        foreach (var criterion in rubric.Criteria)
        {
            totalWeight += criterion.Weight.Value;
            var evaluation = gradebook.GetEvaluation(criterion.Id);
            if (evaluation is not null)
            {
                var normalized = evaluation.Score.Value / criterion.MaxScore.Value;
                weightedRatioSum += normalized * criterion.Weight.Value;
            }
        }

        return totalWeight == 0m ? 0m : (weightedRatioSum / totalWeight) * targetScale;
    }
}

public sealed class WeightedSumStrategy : IGradingStrategy
{
    public string Name => "Прямая взвешенная сумма";

    public decimal Calculate(StudentGradebook gradebook, Rubric rubric)
    {
        decimal sum = 0m;
        foreach (var criterion in rubric.Criteria)
        {
            var evaluation = gradebook.GetEvaluation(criterion.Id);
            if (evaluation is not null)
            {
                sum += (evaluation.Score.Value / criterion.MaxScore.Value) * criterion.Weight.Value;
            }
        }

        return sum;
    }
}