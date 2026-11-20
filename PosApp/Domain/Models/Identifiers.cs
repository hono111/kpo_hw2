namespace PosApp.Domain.Models;

public readonly record struct StudentId(Guid Value)
{
    public static StudentId New() => new(Guid.NewGuid());

    public static StudentId From(Guid value) =>
        value == Guid.Empty
            ? throw new ArgumentException("Идентификатор студента не может быть пустым.", nameof(value))
            : new StudentId(value);

    public override string ToString() => Value.ToString();
}

public readonly record struct CriterionId(Guid Value)
{
    public static CriterionId New() => new(Guid.NewGuid());

    public static CriterionId From(Guid value) =>
        value == Guid.Empty
            ? throw new ArgumentException("Идентификатор критерия не может быть пустым.", nameof(value))
            : new CriterionId(value);

    public override string ToString() => Value.ToString();
}