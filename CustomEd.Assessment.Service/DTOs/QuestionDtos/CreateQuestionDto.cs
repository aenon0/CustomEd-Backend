namespace CustomEd.Assessment.Service.DTOs;

public class CreateQuestionDto
{
    public string Text { get; set; } = null!;
    public List<string> Answers {get; set;} = null!;
    public Guid CorrectAnswerId { get; set; }
    public Guid AssessmentId { get; set; }
    public  List<string>  Tags { get; set; } = null!;
}
