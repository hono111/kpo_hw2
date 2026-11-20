namespace PosApp.Domain.Models;

public sealed class Student
{
    public StudentId Id { get; }
    public string FullName { get; private set; }
    public string Group { get; private set; }

    public Student(StudentId id, string fullName, string group)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("ФИО студента не может быть пустым.", nameof(fullName));

        if (string.IsNullOrWhiteSpace(group))
            throw new ArgumentException("Учебная группа не может быть пустой.", nameof(group));

        Id = id;
        FullName = fullName.Trim();
        Group = group.Trim();
    }

    public void UpdateInfo(string fullName, string group)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("ФИО студента не может быть пустым.", nameof(fullName));

        if (string.IsNullOrWhiteSpace(group))
            throw new ArgumentException("Учебная группа не может быть пустой.", nameof(group));

        FullName = fullName.Trim();
        Group = group.Trim();
    }
}