namespace CustomEd.Assessment.Service.DTOs;
public class CreateAssessmentDto
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Tag {get; set;} = null!;
    public Guid ClassroomId { get; set; }
    public DateTime Deadline { get; set; }

}