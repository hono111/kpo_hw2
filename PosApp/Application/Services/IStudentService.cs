namespace PosApp.Application.Services;

public sealed record AddStudentRequest(string FullName, string Group);
public sealed record StudentDto(Guid Id, string FullName, string Group);

public interface IStudentService
{
    StudentDto Add(AddStudentRequest request);
    IReadOnlyList<StudentDto> List();
    void Update(Guid id, string fullName, string group);
    void Remove(Guid id);
}