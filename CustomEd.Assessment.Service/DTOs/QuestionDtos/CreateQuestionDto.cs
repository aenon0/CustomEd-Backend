namespace CustomEd.Assessment.Service.DTOs;

public class CreateQuestionDto
{
    public string Text { get; set; } = null!;
    public double Weight { get; set; } = 1.0;
    public List<string> Answers {get; set;} = null!;
    public int CorrectAnswerIndex { get; set; }
    public Guid AssessmentId { get; set; }
    public  List<string>  Tags { get; set; } = null!;
}
