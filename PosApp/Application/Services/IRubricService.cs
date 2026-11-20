using PosApp.Domain.Models;

namespace PosApp.Application.Services;

public sealed record AddCriterionRequest(string Title, decimal MaxScore, decimal Weight);
public sealed record UpdateCriterionRequest(Guid CriterionId, string Title, decimal MaxScore, decimal Weight);
public sealed record CriterionDto(Guid Id, string Title, decimal MaxScore, decimal Weight);
public sealed record RubricDto(string CourseName, IReadOnlyList<CriterionDto> Criteria);

public interface IRubricService
{
    RubricDto Get();
    CriterionDto AddCriterion(AddCriterionRequest request);
    void UpdateCriterion(UpdateCriterionRequest request);
    void RemoveCriterion(Guid criterionId);
    void ReplaceRubric(Rubric rubric);
}