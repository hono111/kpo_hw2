namespace PosApp.Domain.Models;

public sealed class StudentGradebook
{
    private readonly Dictionary<CriterionId, Evaluation> _evaluations = [];

    public StudentId StudentId { get; }
    public IReadOnlyCollection<Evaluation> Evaluations => _evaluations.Values;

    public StudentGradebook(StudentId studentId)
    {
        StudentId = studentId;
    }

    public static StudentGradebook Restore(StudentId studentId, IEnumerable<Evaluation> evaluations)
    {
        var gradebook = new StudentGradebook(studentId);
        foreach (var evaluation in evaluations)
            gradebook._evaluations[evaluation.CriterionId] = evaluation;

        return gradebook;
    }

    public void RecordEvaluation(Evaluation evaluation, Rubric rubric)
    {
        ArgumentNullException.ThrowIfNull(evaluation);
        ArgumentNullException.ThrowIfNull(rubric);

        var criterion = rubric.FindCriterion(evaluation.CriterionId)
            ?? throw new InvalidOperationException(
                $"Нельзя выставить оценку: критерий {evaluation.CriterionId} отсутствует в рубрикаторе.");

        if (evaluation.Score.Value > criterion.MaxScore.Value)
            throw new InvalidOperationException(
                $"Балл {evaluation.Score.Value} превышает максимум {criterion.MaxScore.Value} для критерия «{criterion.Title}».");

        _evaluations[evaluation.CriterionId] = evaluation;
    }

    public void CancelEvaluation(CriterionId criterionId)
    {
        if (!_evaluations.Remove(criterionId))
            throw new InvalidOperationException($"Оценка по критерию {criterionId} не найдена и не может быть отменена.");
    }

    public Evaluation? GetEvaluation(CriterionId criterionId) =>
        _evaluations.GetValueOrDefault(criterionId);
}