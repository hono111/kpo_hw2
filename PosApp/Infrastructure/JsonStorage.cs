using System.Text.Json;
using PosApp.Infrastructure.Dtos;

namespace PosApp.Infrastructure;

internal sealed class JsonStorage(string filePath = "pos_data.json") : IDataStorage
{
    private static readonly JsonSerializerOptions Options = new() { WriteIndented = true };

    public PosDatabaseDto Load()
    {
        if (!File.Exists(filePath))
            return new PosDatabaseDto();

        var json = File.ReadAllText(filePath);
        if (string.IsNullOrWhiteSpace(json))
            return new PosDatabaseDto();

        return JsonSerializer.Deserialize<PosDatabaseDto>(json)
            ?? throw new InvalidOperationException($"Не удалось десериализовать хранилище из «{filePath}».");
    }

    public void Save(PosDatabaseDto database)
    {
        var directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directory))
            Directory.CreateDirectory(directory);

        var json = JsonSerializer.Serialize(database, Options);
        File.WriteAllText(filePath, json);
    }
}