using PosApp.Domain.Models;
using PosApp.Domain.Repositories;
using PosApp.Infrastructure.Mappers;

namespace PosApp.Infrastructure;

internal sealed class JsonStudentRepository(IDataStorage storage) : IStudentRepository
{
    public IReadOnlyList<Student> List() =>
        storage.Load().Students.Values.Select(DomainPersistenceMapper.ToDomain).ToList();

    public Student? GetById(StudentId id) =>
        storage.Load().Students.TryGetValue(id.Value, out var dto) ? dto.ToDomain() : null;

    public void Save(Student student)
    {
        var db = storage.Load();
        db.Students[student.Id.Value] = student.ToDto();
        storage.Save(db);
    }

    public void Delete(StudentId id)
    {
        var db = storage.Load();
        db.Students.Remove(id.Value);
        storage.Save(db);
    }
}

internal sealed class JsonRubricRepository(IDataStorage storage) : IRubricRepository
{
    public Rubric Get()
    {
        var db = storage.Load();
        return db.Rubric is null
            ? new Rubric("КПО: Проектирование ПО")
            : db.Rubric.ToDomain();
    }

    public void Save(Rubric rubric)
    {
        var db = storage.Load();
        db.Rubric = rubric.ToDto();
        storage.Save(db);
    }
}

internal sealed class JsonGradebookRepository(IDataStorage storage) : IGradebookRepository
{
    public IReadOnlyList<StudentGradebook> List() =>
        storage.Load().Gradebooks.Values.Select(DomainPersistenceMapper.ToDomain).ToList();

    public StudentGradebook GetOrCreate(StudentId studentId)
    {
        var db = storage.Load();
        return db.Gradebooks.TryGetValue(studentId.Value, out var dto)
            ? dto.ToDomain()
            : new StudentGradebook(studentId);
    }

    public void Save(StudentGradebook gradebook)
    {
        var db = storage.Load();
        db.Gradebooks[gradebook.StudentId.Value] = gradebook.ToDto();
        storage.Save(db);
    }

    public void DeleteByStudentId(StudentId studentId)
    {
        var db = storage.Load();
        db.Gradebooks.Remove(studentId.Value);
        storage.Save(db);
    }
}