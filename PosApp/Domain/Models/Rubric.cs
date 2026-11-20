namespace PosApp.Domain.Models;

public sealed class Rubric
{
    private readonly List<Criterion> _criteria = [];

    public string CourseName { get; }
    public IReadOnlyList<Criterion> Criteria => _criteria.AsReadOnly();

    public Rubric(string courseName, IEnumerable<Criterion>? criteria = null)
    {
        if (string.IsNullOrWhiteSpace(courseName))
            throw new ArgumentException("Название курса не может быть пустым.", nameof(courseName));

        CourseName = courseName.Trim();

        if (criteria is not null)
        {
            foreach (var criterion in criteria)
                AddCriterion(criterion);
        }
    }

    public static Rubric Restore(string courseName, IEnumerable<Criterion> criteria)
    {
        var rubric = new Rubric(courseName);
        rubric._criteria.AddRange(criteria);
        return rubric;
    }

    public Criterion AddCriterion(string title, MaxScore maxScore, Weight weight)
    {
        var criterion = new Criterion(CriterionId.New(), title, maxScore, weight);
        AddCriterion(criterion);
        return criterion;
    }

    public void AddCriterion(Criterion criterion)
    {
        ArgumentNullException.ThrowIfNull(criterion);

        if (_criteria.Any(c => string.Equals(c.Title, criterion.Title, StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException($"Критерий с названием «{criterion.Title}» уже существует в рубрикаторе.");

        _criteria.Add(criterion);
    }

    public void UpdateCriterion(CriterionId id, string title, MaxScore maxScore, Weight weight)
    {
        var criterion = FindCriterion(id)
            ?? throw new InvalidOperationException($"Критерий {id} не найден в рубрикаторе.");

        if (_criteria.Any(c => c.Id != id && string.Equals(c.Title, title.Trim(), StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException($"Критерий с названием «{title}» уже существует в рубрикаторе.");

        criterion.Update(title, maxScore, weight);
    }

    public void RemoveCriterion(CriterionId id)
    {
        var criterion = FindCriterion(id)
            ?? throw new InvalidOperationException($"Критерий {id} не найден в рубрикаторе.");

        _criteria.Remove(criterion);
    }

    public Criterion? FindCriterion(CriterionId id) =>
        _criteria.FirstOrDefault(c => c.Id == id);

    public bool ContainsCriterion(CriterionId id) =>
        _criteria.Any(c => c.Id == id);
}