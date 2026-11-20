using System.Globalization;

namespace PosApp.Presentation;

internal sealed class ConsoleUi(IPosConsoleFacade facade) : IConsoleUi
{
    public void Run(bool autoDemoOnly = false)
    {
        Console.WriteLine("=== Модуль ПОС: Преподавательская Оценка Студентов (ФКН ВШЭ) ===");

        if (autoDemoOnly)
        {
            ExecuteDemo();
            return;
        }

        PrintHelp();

        while (true)
        {
            Console.Write("\npos> ");
            var command = Console.ReadLine()?.Trim();
            if (string.IsNullOrWhiteSpace(command))
                continue;

            try
            {
                switch (command.ToLowerInvariant())
                {
                    case "help": PrintHelp(); break;
                    case "students": ShowStudents(); break;
                    case "add student": AddStudent(); break;
                    case "del student": DeleteStudent(); break;
                    case "rubric": ShowRubric(); break;
                    case "add criterion": AddCriterion(); break;
                    case "edit criterion": EditCriterion(); break;
                    case "del criterion": DeleteCriterion(); break;
                    case "grade": GradeStudent(); break;
                    case "cancel grade": CancelGrade(); break;
                    case "summary": ShowSummary(); break;
                    case "export": Export(); break;
                    case "demo": ExecuteDemo(); break;
                    case "exit": return;
                    default:
                        Console.WriteLine($"Неизвестная команда: «{command}». Введите help для списка команд.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Ошибка домена/приложения]: {ex.Message}");
            }
        }
    }

    private static void PrintHelp() => Console.WriteLine("""
        Доступные команды:
          students        — список студентов
          add student     — зарегистрировать студента
          del student     — удалить студента
          rubric          — показать рубрикатор курса
          add criterion   — добавить критерий в рубрикатор
          edit criterion  — изменить критерий
          del criterion   — удалить критерий
          grade           — выставить / исправить оценку студенту
          cancel grade    — отменить оценку студента по критерию
          summary         — показать оценки и итоговый балл
          export          — экспортировать итоговую ведомость (csv / json)
          demo            — прогнать сквозной демонстрационный сценарий
          help            — показать справку
          exit            — выход
        """);

    private void ShowStudents()
    {
        var students = facade.GetStudents();
        if (students.Count == 0) { Console.WriteLine("Список студентов пуст."); return; }
        foreach (var s in students)
            Console.WriteLine($"  [{s.Id}] {s.FullName} — группа {s.Group}");
    }

    private void AddStudent()
    {
        Console.Write("ФИО студента: ");
        var name = Console.ReadLine() ?? string.Empty;
        Console.Write("Группа: ");
        var group = Console.ReadLine() ?? string.Empty;
        var created = facade.RegisterStudent(name, group);
        Console.WriteLine($"Студент добавлен. ID: {created.Id}");
    }

    private void DeleteStudent()
    {
        facade.DeleteStudent(ReadGuid("ID студента: "));
        Console.WriteLine("Студент удалён.");
    }

    private void ShowRubric()
    {
        var rubric = facade.GetRubric();
        Console.WriteLine($"Курс: {rubric.CourseName}");
        if (rubric.Criteria.Count == 0) { Console.WriteLine("  Критерии ещё не заведены."); return; }
        foreach (var c in rubric.Criteria)
            Console.WriteLine($"  [{c.Id}] {c.Title} | max: {c.MaxScore} | вес: {c.Weight}");
    }

    private void AddCriterion()
    {
        Console.Write("Название критерия: ");
        var title = Console.ReadLine() ?? string.Empty;
        var created = facade.AddCriterion(title, ReadDecimal("Максимальный балл: "), ReadDecimal("Вес критерия: "));
        Console.WriteLine($"Критерий добавлен. ID: {created.Id}");
    }

    private void EditCriterion()
    {
        var id = ReadGuid("ID критерия: ");
        Console.Write("Новое название: ");
        var title = Console.ReadLine() ?? string.Empty;
        facade.UpdateCriterion(id, title, ReadDecimal("Новый максимальный балл: "), ReadDecimal("Новый вес: "));
        Console.WriteLine("Критерий обновлён.");
    }

    private void DeleteCriterion()
    {
        facade.DeleteCriterion(ReadGuid("ID критерия: "));
        Console.WriteLine("Критерий удалён.");
    }

    private void GradeStudent()
    {
        var studentId = ReadGuid("ID студента: ");
        var criterionId = ReadGuid("ID критерия: ");
        var score = ReadDecimal("Балл: ");
        Console.Write("Комментарий (необязательно): ");
        facade.GradeStudent(studentId, criterionId, score, Console.ReadLine());
        Console.WriteLine("Оценка сохранена.");
    }

    private void CancelGrade()
    {
        facade.CancelGrade(ReadGuid("ID студента: "), ReadGuid("ID критерия: "));
        Console.WriteLine("Оценка отменена.");
    }

    private void ShowSummary()
    {
        Console.Write("Стратегия расчёта (normalized / sum, по умолчанию normalized): ");
        var strategy = Console.ReadLine()?.Trim();
        if (string.IsNullOrWhiteSpace(strategy)) strategy = "normalized";

        var summaries = facade.GetAllSummaries(strategy);
        if (summaries.Count == 0) { Console.WriteLine("Нет студентов для расчёта."); return; }

        foreach (var s in summaries)
        {
            Console.WriteLine($"\nСтудент: {s.StudentName} ({s.Group}) [{s.StudentId}]");
            if (s.Evaluations.Count == 0)
            {
                Console.WriteLine("  Оценок пока нет.");
            }
            else
            {
                foreach (var e in s.Evaluations)
                {
                    var commentPart = e.Comment is null ? string.Empty : $" — «{e.Comment}»";
                    Console.WriteLine($"  • {e.CriterionTitle}: {e.Score}/{e.MaxScore} (вес {e.Weight}){commentPart}");
                }
            }
            Console.WriteLine($"  ИТОГ ({s.StrategyName}): {s.FinalScore:0.00}");
        }
    }

    private void Export()
    {
        Console.Write("Формат (csv / json): ");
        var format = Console.ReadLine()?.Trim() ?? "csv";
        Console.Write("Путь к файлу (например, report.csv): ");
        var path = Console.ReadLine()?.Trim() ?? $"report.{format}";
        Console.Write("Стратегия (normalized / sum, по умолчанию normalized): ");
        var strategy = Console.ReadLine()?.Trim();
        if (string.IsNullOrWhiteSpace(strategy)) strategy = "normalized";

        Console.WriteLine($"Отчёт сохранён: {facade.ExportReport(format, path, strategy)}");
    }

    private void ExecuteDemo()
    {
        Console.WriteLine("\n--- Запуск сквозного демонстрационного сценария ПОС ---");
        foreach (var line in facade.RunEndToEndDemo())
            Console.WriteLine(line);
        Console.WriteLine("--- Демонстрация завершена ---");
    }

    private static Guid ReadGuid(string prompt)
    {
        Console.Write(prompt);
        return Guid.TryParse(Console.ReadLine()?.Trim(), out var id)
            ? id
            : throw new FormatException("Некорректный формат GUID.");
    }

    private static decimal ReadDecimal(string prompt)
    {
        Console.Write(prompt);
        var input = Console.ReadLine()?.Trim()?.Replace(',', '.');
        return decimal.TryParse(input, NumberStyles.Number, CultureInfo.InvariantCulture, out var value)
            ? value
            : throw new FormatException("Некорректный числовой формат.");
    }
}