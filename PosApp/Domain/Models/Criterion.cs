namespace PosApp.Domain.Models;

public sealed class Criterion
{
    public CriterionId Id { get; }
    public string Title { get; private set; }
    public MaxScore MaxScore { get; private set; }
    public Weight Weight { get; private set; }

    public Criterion(CriterionId id, string title, MaxScore maxScore, Weight weight)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Название критерия не может быть пустым.", nameof(title));

        Id = id;
        Title = title.Trim();
        MaxScore = maxScore ?? throw new ArgumentNullException(nameof(maxScore));
        Weight = weight ?? throw new ArgumentNullException(nameof(weight));
    }

    internal void Update(string title, MaxScore maxScore, Weight weight)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Название критерия не может быть пустым.", nameof(title));

        Title = title.Trim();
        MaxScore = maxScore ?? throw new ArgumentNullException(nameof(maxScore));
        Weight = weight ?? throw new ArgumentNullException(nameof(weight));
    }
}