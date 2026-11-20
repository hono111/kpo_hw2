using PosApp.Application.Services;

namespace PosApp.Application.Export;

public abstract class StatementExporterBase(IEvaluationService evaluationService) : IStatementExporter
{
    public abstract string Format { get; }

    public string Export(string filePath, string strategyKey = "normalized")
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("Путь к файлу экспорта не может быть пустым.", nameof(filePath));

        var summaries = evaluationService.GetAllSummaries(strategyKey);
        var content = FormatContent(summaries);

        var directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directory))
            Directory.CreateDirectory(directory);

        File.WriteAllText(filePath, content);
        return Path.GetFullPath(filePath);
    }

    protected abstract string FormatContent(IReadOnlyList<StudentSummaryDto> summaries);
}