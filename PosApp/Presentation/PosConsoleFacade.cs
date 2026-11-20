using PosApp.Application.Export;
using PosApp.Application.Services;
using PosApp.Domain.Creation;

namespace PosApp.Presentation;

internal sealed class PosConsoleFacade(
    IStudentService studentService,
    IRubricService rubricService,
    IEvaluationService evaluationService,
    IEnumerable<IStatementExporter> exporters) : IPosConsoleFacade
{
    public StudentDto RegisterStudent(string fullName, string group) =>
        studentService.Add(new AddStudentRequest(fullName, group));

    public IReadOnlyList<StudentDto> GetStudents() => studentService.List();

    public void DeleteStudent(Guid id) => studentService.Remove(id);

    public RubricDto GetRubric() => rubricService.Get();

    public CriterionDto AddCriterion(string title, decimal maxScore, decimal weight) =>
        rubricService.AddCriterion(new AddCriterionRequest(title, maxScore, weight));

    public void UpdateCriterion(Guid id, string title, decimal maxScore, decimal weight) =>
        rubricService.UpdateCriterion(new UpdateCriterionRequest(id, title, maxScore, weight));

    public void DeleteCriterion(Guid id) => rubricService.RemoveCriterion(id);

    public void GradeStudent(Guid studentId, Guid criterionId, decimal score, string? comment = null) =>
        evaluationService.RecordOrUpdate(new RecordEvaluationRequest(studentId, criterionId, score, comment));

    public void CancelGrade(Guid studentId, Guid criterionId) =>
        evaluationService.Cancel(studentId, criterionId);

    public StudentSummaryDto GetStudentSummary(Guid studentId, string strategy = "normalized") =>
        evaluationService.GetStudentSummary(studentId, strategy);

    public IReadOnlyList<StudentSummaryDto> GetAllSummaries(string strategy = "normalized") =>
        evaluationService.GetAllSummaries(strategy);

    public string ExportReport(string format, string filePath, string strategy = "normalized")
    {
        var exporter = exporters.FirstOrDefault(e => string.Equals(e.Format, format, StringComparison.OrdinalIgnoreCase))
            ?? throw new ArgumentException($"Неподдерживаемый формат экспорта: «{format}». Доступны: csv, json.");

        return exporter.Export(filePath, strategy);
    }

    public IReadOnlyList<string> RunEndToEndDemo(string outputDirectory = "demo_output")
    {
        var log = new List<string>();

        var rubric = new RubricBuilder()
            .ForCourse("КПО: Проектирование ПО (ФКН ВШЭ)")
            .AddCriterion("Семинары и активность", maxScore: 10m, weight: 0.2m)
            .AddCriterion("Домашнее задание 1 (SOLID)", maxScore: 10m, weight: 0.3m)
            .AddCriterion("Домашнее задание 2 (DDD + GoF)", maxScore: 10m, weight: 0.5m)
            .Build();

        rubricService.ReplaceRubric(rubric);
        log.Add($"[1/5] Сформирован рубрикатор «{rubric.CourseName}» ({rubric.Criteria.Count} критерия).");

        var s1 = RegisterStudent("Иванов Алексей", "БПИ-251");
        var s2 = RegisterStudent("Смирнова Мария", "БПИ-252");
        log.Add($"[2/5] Зарегистрированы студенты: {s1.FullName} ({s1.Id}), {s2.FullName} ({s2.Id}).");

        var cSem = rubric.Criteria[0].Id.Value;
        var cHw1 = rubric.Criteria[1].Id.Value;
        var cHw2 = rubric.Criteria[2].Id.Value;

        GradeStudent(s1.Id, cSem, 8.5m, "Активная работа у доски");
        GradeStudent(s1.Id, cHw1, 9.0m, "Чистый SOLID");
        GradeStudent(s1.Id, cHw2, 9.5m, "Отличная доменная модель");

        GradeStudent(s2.Id, cSem, 10.0m, "Максимум за семинары");
        GradeStudent(s2.Id, cHw1, 7.5m, "Есть замечания по тестам");
        GradeStudent(s2.Id, cHw2, 8.0m, "Хорошая архитектура");

        try
        {
            GradeStudent(s1.Id, cHw2, 12.0m, "Попытка нарушить диапазон");
        }
        catch (Exception ex)
        {
            log.Add($"[3/5] Инвариант сработал штатно при попытке поставить 12/10: {ex.Message}");
        }

        foreach (var summary in GetAllSummaries("normalized"))
        {
            log.Add($"[4/5] Итог ({summary.StrategyName}) — {summary.StudentName}: {summary.FinalScore:0.00} баллов.");
        }

        var csvPath = ExportReport("csv", Path.Combine(outputDirectory, "pos_report.csv"), "normalized");
        var jsonPath = ExportReport("json", Path.Combine(outputDirectory, "pos_report.json"), "normalized");
        log.Add($"[5/5] Ведомости экспортированы:\n      CSV:  {csvPath}\n      JSON: {jsonPath}");

        return log;
    }
}