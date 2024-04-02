
using CustomEd.Assessment.Service.Model;

public class QuestionDto
{
    public Guid Id;
    public string Text { get; set; } = null!;
    public List<Answer> Answers { get; set; } = new List<Answer>();
    public Guid AssessmentId { get; set; }
    public List<string> Tags { get; set; } = null!;
    public  DateTime CreatedAt { get; set; }
    public  DateTime UpdatedAt { get; set; }

}