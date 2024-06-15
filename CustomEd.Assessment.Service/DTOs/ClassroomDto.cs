
using CustomEd.Shared.Model;

namespace CustomEd.Assessment.Service.Model;

public class ClassroomDto
{

    public Guid Id {get; set;}
    public string Name { get; set; } = null!;
    public string CourseNo { get; set; } = null!;
    public string Description { get; set; } = null!;
    public Guid CreatorId { get; set; }
    public List<Guid> MemberIds {get; set;} = null!;
    

}