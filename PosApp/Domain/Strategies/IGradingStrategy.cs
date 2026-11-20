using PosApp.Domain.Models;

namespace PosApp.Domain.Strategies;

public interface IGradingStrategy
{
    string Name { get; }
    decimal Calculate(StudentGradebook gradebook, Rubric rubric);
}