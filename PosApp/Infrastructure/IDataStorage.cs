using PosApp.Infrastructure.Dtos;

namespace PosApp.Infrastructure;

internal interface IDataStorage
{
    PosDatabaseDto Load();
    void Save(PosDatabaseDto database);
}