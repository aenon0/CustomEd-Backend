using CustomEd.Shared.Model;

namespace CustomEd.Assessment.Service.Model;

public class Question : BaseEntity
{
    public string Text { get; set; } = null!;
    public List<Answer> Answers { get; set; } = new List<Answer>();
    public Guid CorrectAnswerId { get; set; }
    public Guid AssessmentId { get; set; }
    public string Tags { get; set; } = null!;
}