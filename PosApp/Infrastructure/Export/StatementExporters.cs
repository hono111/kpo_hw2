using System.Globalization;
using System.Text;
using System.Text.Json;
using PosApp.Application.Export;
using PosApp.Application.Services;

namespace PosApp.Infrastructure.Export;

internal sealed class CsvStatementExporter(IEvaluationService evaluationService)
    : StatementExporterBase(evaluationService)
{
    public override string Format => "csv";

    protected override string FormatContent(IReadOnlyList<StudentSummaryDto> summaries)
    {
        var sb = new StringBuilder();
        sb.AppendLine("StudentId;StudentName;Group;EvaluatedCriteria;FinalScore;Strategy");

        foreach (var s in summaries)
        {
            var details = string.Join(", ", s.Evaluations.Select(e =>
                $"{e.CriterionTitle}: {e.Score.ToString(CultureInfo.InvariantCulture)}/{e.MaxScore.ToString(CultureInfo.InvariantCulture)}"));

            sb.AppendLine(string.Join(';',
                s.StudentId,
                EscapeCsv(s.StudentName),
                EscapeCsv(s.Group),
                EscapeCsv(details),
                s.FinalScore.ToString("0.00", CultureInfo.InvariantCulture),
                EscapeCsv(s.StrategyName)));
        }

        return sb.ToString();
    }

    private static string EscapeCsv(string value) =>
        value.Contains(';') || value.Contains('"')
            ? $"\"{value.Replace("\"", "\"\"")}\""
            : value;
}

internal sealed class JsonStatementExporter(IEvaluationService evaluationService)
    : StatementExporterBase(evaluationService)
{
    private static readonly JsonSerializerOptions Options = new() { WriteIndented = true };

    public override string Format => "json";

    protected override string FormatContent(IReadOnlyList<StudentSummaryDto> summaries) =>
        JsonSerializer.Serialize(summaries, Options);
}