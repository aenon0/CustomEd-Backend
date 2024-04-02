using CustomEd.Shared.Model;

namespace CustomEd.Assessment.Service.Model;

public class Assessment: BaseEntity
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Tag {get; set;} = null!;
    public List<Question> Questions { get; set; } = null!; 
    public Classroom Classroom { get; set; } = null!;
    public DateTime Deadline { get; set; }

}