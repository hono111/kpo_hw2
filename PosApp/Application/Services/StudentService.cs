using PosApp.Domain.Models;
using PosApp.Domain.Repositories;

namespace PosApp.Application.Services;

internal sealed class StudentService(
    IStudentRepository studentRepository,
    IGradebookRepository gradebookRepository) : IStudentService
{
    public StudentDto Add(AddStudentRequest request)
    {
        var student = new Student(StudentId.New(), request.FullName, request.Group);
        studentRepository.Save(student);
        gradebookRepository.GetOrCreate(student.Id);
        return new StudentDto(student.Id.Value, student.FullName, student.Group);
    }

    public IReadOnlyList<StudentDto> List() =>
        studentRepository.List()
            .Select(s => new StudentDto(s.Id.Value, s.FullName, s.Group))
            .ToList();

    public void Update(Guid id, string fullName, string group)
    {
        var studentId = StudentId.From(id);
        var student = studentRepository.GetById(studentId)
            ?? throw new InvalidOperationException($"Студент с ID {id} не найден.");

        student.UpdateInfo(fullName, group);
        studentRepository.Save(student);
    }

    public void Remove(Guid id)
    {
        var studentId = StudentId.From(id);
        if (studentRepository.GetById(studentId) is null)
            throw new InvalidOperationException($"Студент с ID {id} не найден.");

        gradebookRepository.DeleteByStudentId(studentId);
        studentRepository.Delete(studentId);
    }
}