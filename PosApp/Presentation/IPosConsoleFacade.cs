using PosApp.Application.Services;

namespace PosApp.Presentation;

public interface IPosConsoleFacade
{
    StudentDto RegisterStudent(string fullName, string group);
    IReadOnlyList<StudentDto> GetStudents();
    void DeleteStudent(Guid id);

    RubricDto GetRubric();
    CriterionDto AddCriterion(string title, decimal maxScore, decimal weight);
    void UpdateCriterion(Guid id, string title, decimal maxScore, decimal weight);
    void DeleteCriterion(Guid id);

    void GradeStudent(Guid studentId, Guid criterionId, decimal score, string? comment = null);
    void CancelGrade(Guid studentId, Guid criterionId);
    StudentSummaryDto GetStudentSummary(Guid studentId, string strategy = "normalized");
    IReadOnlyList<StudentSummaryDto> GetAllSummaries(string strategy = "normalized");

    string ExportReport(string format, string filePath, string strategy = "normalized");
    IReadOnlyList<string> RunEndToEndDemo(string outputDirectory = "demo_output");
}