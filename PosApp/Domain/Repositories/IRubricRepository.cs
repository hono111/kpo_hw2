using PosApp.Domain.Models;

namespace PosApp.Domain.Repositories;

public interface IRubricRepository
{
    Rubric Get();
    void Save(Rubric rubric);
}