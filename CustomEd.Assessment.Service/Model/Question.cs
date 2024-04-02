using CustomEd.Shared.Model;

namespace CustomEd.Assessment.Service.Model;

public class Question : BaseEntity
{
    public string Text { get; set; } = null!;
    public double Weight { get; set; } = 1.0;
    public List<Answer> Answers { get; set; } = new List<Answer>();
    public Guid AssessmentId { get; set; }
    public List<string> Tags { get; set; } = null!;
}