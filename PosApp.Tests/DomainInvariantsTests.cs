using PosApp.Domain.Creation;
using PosApp.Domain.Models;
using Xunit;

namespace PosApp.Tests;

public class DomainInvariantsTests
{
    [Theory]
    [MemberData(nameof(TestData.OutOfRangeScores), MemberType = typeof(TestData))]
    public void Score_CreateOutOfRange_ShouldThrow(decimal rawScore)
    {
        // Arrange
        var max = new MaxScore(10m);

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => new Score(rawScore, max));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(5.5)]
    [InlineData(10)]
    public void Score_CreateWithinRange_ShouldSucceed(decimal rawScore)
    {
        // Arrange
        var max = new MaxScore(10m);

        // Act
        var score = new Score(rawScore, max);

        // Assert
        Assert.Equal(rawScore, score.Value);
    }

    [Fact]
    public void EvaluationFactory_CreateForUnknownCriterion_ShouldThrow()
    {
        // Arrange
        var rubric = new RubricBuilder()
            .ForCourse("КПО: Проектирование ПО")
            .AddCriterion("ДЗ-1", maxScore: 10m, weight: 1m)
            .Build();
        var factory = new EvaluationFactory(TimeProvider.System);
        var unknownCriterionId = CriterionId.New();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() =>
            factory.Create(rubric, unknownCriterionId, 8m));
    }

    [Fact]
    public void Rubric_AddDuplicateCriterionTitle_ShouldThrow()
    {
        // Arrange
        var rubric = new Rubric("КПО");
        rubric.AddCriterion("Семинары", new MaxScore(10m), new Weight(0.3m));

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() =>
            rubric.AddCriterion("семинары", new MaxScore(10m), new Weight(0.5m)));
    }

    [Fact]
    public void RubricBuilder_BuildWithoutCriteria_ShouldThrow()
    {
        // Arrange
        var builder = new RubricBuilder().ForCourse("Пустой курс");

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => builder.Build());
    }

    [Fact]
    public void StudentGradebook_CancelMissingEvaluation_ShouldThrow()
    {
        // Arrange
        var gradebook = new StudentGradebook(StudentId.New());
        var missingCriterionId = CriterionId.New();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() =>
            gradebook.CancelEvaluation(missingCriterionId));
    }
}

file static class TestData
{
    public static TheoryData<decimal> OutOfRangeScores => new()
    {
        { -0.01m },
        { -5m },
        { 10.01m },
        { 12m }
    };
}