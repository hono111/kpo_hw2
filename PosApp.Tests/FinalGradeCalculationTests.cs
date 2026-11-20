using PosApp.Domain.Creation;
using PosApp.Domain.Models;
using PosApp.Domain.Services;
using PosApp.Domain.Strategies;
using Xunit;

namespace PosApp.Tests;

public class FinalGradeCalculationTests
{
    [Fact]
    public void CalculateFinalGrade_WithStrategiesAndRoundingDecorator_ShouldReturnExpectedValues()
    {
        // Arrange
        var rubric = new RubricBuilder()
            .ForCourse("КПО")
            .AddCriterion("К1", maxScore: 10m, weight: 1m)
            .AddCriterion("К2", maxScore: 20m, weight: 2m)
            .Build();

        var factory = new EvaluationFactory(TimeProvider.System);
        var gradebook = new StudentGradebook(StudentId.New());

        // 8/10 = 0.8 (вес 1), 15/20 = 0.75 (вес 2)
        gradebook.RecordEvaluation(factory.Create(rubric, rubric.Criteria[0].Id, 8m), rubric);
        gradebook.RecordEvaluation(factory.Create(rubric, rubric.Criteria[1].Id, 15m), rubric);

        // Нормированный итог: ((0.8 * 1 + 0.75 * 2) / 3) * 10 = 7.6666... -> 7.67
        // Прямая сумма: 0.8 * 1 + 0.75 * 2 = 2.30
        var calculator = new FinalGradeCalculationService().WithRounding(decimals: 2);

        // Act
        var normalizedGrade = calculator.CalculateFinalGrade(gradebook, rubric, new WeightedNormalizedStrategy(10m));
        var sumGrade = calculator.CalculateFinalGrade(gradebook, rubric, new WeightedSumStrategy());

        // Assert
        Assert.Equal(7.67m, normalizedGrade.Value);
        Assert.Equal(2.30m, sumGrade.Value);
        Assert.Equal(2, normalizedGrade.EvaluatedCriteriaCount);
        Assert.Equal(2, normalizedGrade.TotalCriteriaCount);
    }

    [Fact]
    public void CalculateFinalGrade_WhenNoEvaluationsRecorded_ShouldReturnZero()
    {
        // Arrange
        var rubric = new RubricBuilder()
            .ForCourse("КПО")
            .AddCriterion("Экзамен", maxScore: 10m, weight: 1m)
            .Build();
        var gradebook = new StudentGradebook(StudentId.New());
        var calculator = new FinalGradeCalculationService();

        // Act
        var result = calculator.CalculateFinalGrade(gradebook, rubric, new WeightedNormalizedStrategy(10m));

        // Assert
        Assert.Equal(0m, result.Value);
        Assert.Equal(0, result.EvaluatedCriteriaCount);
    }
}