namespace PosApp.Domain.Models;

public sealed record MaxScore
{
    public decimal Value { get; }

    public MaxScore(decimal value)
    {
        if (value <= 0m)
            throw new ArgumentOutOfRangeException(nameof(value), "Максимальный балл критерия должен быть больше 0.");

        Value = value;
    }

    public override string ToString() => Value.ToString("0.##");
}


public sealed record Weight
{
    public decimal Value { get; }

    public Weight(decimal value)
    {
        if (value <= 0m)
            throw new ArgumentOutOfRangeException(nameof(value), "Вес критерия должен быть больше 0.");

        Value = value;
    }

    public override string ToString() => Value.ToString("0.##");
}


public sealed record Score
{
    public decimal Value { get; }

    public Score(decimal value, MaxScore maxScore)
    {
        ArgumentNullException.ThrowIfNull(maxScore);

        if (value < 0m || value > maxScore.Value)
            throw new ArgumentOutOfRangeException(
                nameof(value),
                $"Балл {value} выходит за допустимые пределы критерия [0, {maxScore.Value}].");

        Value = value;
    }

    internal static Score Restore(decimal value) => new(value);

    private Score(decimal value) => Value = value;

    public override string ToString() => Value.ToString("0.##");
}


public sealed record Evaluation
{
    public CriterionId CriterionId { get; }
    public Score Score { get; }
    public DateTimeOffset EvaluatedAt { get; }
    public string? Comment { get; }

    internal Evaluation(CriterionId criterionId, Score score, DateTimeOffset evaluatedAt, string? comment)
    {
        CriterionId = criterionId;
        Score = score ?? throw new ArgumentNullException(nameof(score));
        EvaluatedAt = evaluatedAt;
        Comment = string.IsNullOrWhiteSpace(comment) ? null : comment.Trim();
    }

    public static Evaluation Restore(CriterionId criterionId, decimal rawScore, DateTimeOffset evaluatedAt, string? comment) =>
        new(criterionId, Score.Restore(rawScore), evaluatedAt, comment);
}


public sealed record FinalGrade(
    StudentId StudentId,
    decimal Value,
    string StrategyName,
    int EvaluatedCriteriaCount,
    int TotalCriteriaCount);