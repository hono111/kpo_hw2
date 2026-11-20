using PosApp.Domain.Models;
using PosApp.Domain.Strategies;

namespace PosApp.Domain.Services;

public interface IFinalGradeCalculator
{
    FinalGrade CalculateFinalGrade(StudentGradebook gradebook, Rubric rubric, IGradingStrategy strategy);
}