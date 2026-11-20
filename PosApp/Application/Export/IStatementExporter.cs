namespace PosApp.Application.Export;

public interface IStatementExporter
{
    string Format { get; }
    string Export(string filePath, string strategyKey = "normalized");
}