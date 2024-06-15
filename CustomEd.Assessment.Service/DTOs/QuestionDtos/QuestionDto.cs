
using CustomEd.Assessment.Service.DTOs;
using CustomEd.Assessment.Service.Model;

public class QuestionDto
{
    public Guid Id {get; set;}
    public string Text { get; set; } = null!;
    public List<AnswerDto> Answers { get; set; } = new List<AnswerDto>();
    public Guid AssessmentId { get; set; }
    public List<string>? Tags { get; set; }
    public  DateTime CreatedAt { get; set; }
    public  DateTime UpdatedAt { get; set; }

}