using PosApp.Domain.Models;

namespace PosApp.Domain.Repositories;

public interface IGradebookRepository
{
    IReadOnlyList<StudentGradebook> List();
    StudentGradebook GetOrCreate(StudentId studentId);
    void Save(StudentGradebook gradebook);
    void DeleteByStudentId(StudentId studentId);
}