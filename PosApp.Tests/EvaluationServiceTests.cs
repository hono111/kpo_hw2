using NSubstitute;
using PosApp.Application.Services;
using PosApp.Domain.Creation;
using PosApp.Domain.Models;
using PosApp.Domain.Repositories;
using PosApp.Domain.Services;
using PosApp.Domain.Strategies;
using Xunit;

namespace PosApp.Tests;

public class EvaluationServiceTests
{
    [Fact]
    public void RecordOrUpdate_ValidData_ShouldSaveGradebook()
    {
        // Arrange
        var studentId = StudentId.New();
        var student = new Student(studentId, "Иванов Иван", "БПИ-251");
        var rubric = new RubricBuilder()
            .ForCourse("КПО")
            .AddCriterion("Домашнее задание", maxScore: 10m, weight: 1m)
            .Build();
        var criterionId = rubric.Criteria[0].Id;
        var gradebook = new StudentGradebook(studentId);

        var studentRepo = Substitute.For<IStudentRepository>();
        studentRepo.GetById(studentId).Returns(student);

        var rubricRepo = Substitute.For<IRubricRepository>();
        rubricRepo.Get().Returns(rubric);

        var gradebookRepo = Substitute.For<IGradebookRepository>();
        gradebookRepo.GetOrCreate(studentId).Returns(gradebook);

        var service = new EvaluationService(
            studentRepo,
            rubricRepo,
            gradebookRepo,
            new EvaluationFactory(TimeProvider.System),
            new FinalGradeCalculationService(),
            new Dictionary<string, IGradingStrategy>
            {
                ["normalized"] = new WeightedNormalizedStrategy()
            });

        // Act
        service.RecordOrUpdate(new RecordEvaluationRequest(studentId.Value, criterionId.Value, 9.5m, "Отлично"));

        // Assert
        gradebookRepo.Received(1).Save(gradebook);
        Assert.Equal(9.5m, gradebook.GetEvaluation(criterionId)?.Score.Value);
    }

    [Fact]
    public void RecordOrUpdate_StudentNotFound_ShouldThrowAndNotSave()
    {
        // Arrange
        var missingStudentId = StudentId.New();
        var studentRepo = Substitute.For<IStudentRepository>();
        studentRepo.GetById(missingStudentId).Returns((Student?)null);

        var rubricRepo = Substitute.For<IRubricRepository>();
        var gradebookRepo = Substitute.For<IGradebookRepository>();

        var service = new EvaluationService(
            studentRepo,
            rubricRepo,
            gradebookRepo,
            new EvaluationFactory(TimeProvider.System),
            new FinalGradeCalculationService(),
            new Dictionary<string, IGradingStrategy>());

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() =>
            service.RecordOrUpdate(new RecordEvaluationRequest(missingStudentId.Value, Guid.NewGuid(), 8m)));

        gradebookRepo.DidNotReceive().Save(Arg.Any<StudentGradebook>());
    }
}