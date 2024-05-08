using CustomEd.Shared.Model;

namespace CustomEd.Classroom.Service.Model;

public class Classroom : BaseEntity
{
    public Classroom()
    {
        Members = new List<Student>();
    }
    public string Name { get; set; } = null!;
    public string CourseNo { get; set; } = null!;
    public string Description { get; set; } = null!;
    public Teacher Creator { get; set; } = null!;
    public List<Student> Members {get; set;}
}
