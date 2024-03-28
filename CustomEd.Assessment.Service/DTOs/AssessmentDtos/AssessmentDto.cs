using CustomEd.Assessment.Service.Model;

namespace CustomEd.Assessment.Service.DTOs;
public class AssessmentDto
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Tag { get; set; } = null!;
    public List<QuestionDto> Questions { get; set; } = null!;
    public ClassroomDto Classroom { get; set; } = null!;
    public DateTime Deadline { get; set; }
}