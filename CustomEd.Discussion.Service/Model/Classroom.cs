using CustomEd.Shared.Model;

namespace CustomEd.Assessment.Service.Model;

public class Classroom : BaseEntity
{

    public string Name { get; set; } = null!;
    public string CourseNo { get; set; } = null!;
    public string Description { get; set; } = null!;
    public Guid CreatorId { get; set; }
    public List<Guid> Members {get; set;} = null!;
    

}