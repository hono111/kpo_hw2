using PosApp.Domain.Models;

namespace PosApp.Domain.Repositories;

public interface IStudentRepository
{
    IReadOnlyList<Student> List();
    Student? GetById(StudentId id);
    void Save(Student student);
    void Delete(StudentId id);
}