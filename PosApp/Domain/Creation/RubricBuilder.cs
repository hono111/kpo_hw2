using PosApp.Domain.Models;

namespace PosApp.Domain.Creation;

public sealed class RubricBuilder
{
    private string _courseName = "КПО: Проектирование ПО";
    private readonly List<(string Title, decimal MaxScore, decimal Weight)> _items = [];

    public RubricBuilder ForCourse(string courseName)
    {
        _courseName = courseName;
        return this;
    }

    public RubricBuilder AddCriterion(string title, decimal maxScore, decimal weight)
    {
        _items.Add((title, maxScore, weight));
        return this;
    }

    public Rubric Build()
    {
        if (_items.Count == 0)
            throw new InvalidOperationException("Невозможно собрать пустой рубрикатор: добавьте хотя бы один критерий.");

        var rubric = new Rubric(_courseName);
        foreach (var (title, maxScore, weight) in _items)
        {
            rubric.AddCriterion(title, new MaxScore(maxScore), new Weight(weight));
        }

        return rubric;
    }
}