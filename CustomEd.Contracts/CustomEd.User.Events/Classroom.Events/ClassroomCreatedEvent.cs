namespace CustomEd;

public class ClassroomCreatedEvent
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string CourseNo { get; set; } = null!;
    public string Description { get; set; } = null!;
    public Teacher Creator { get; set; } = null!;
    public List<Student> Members {get; set;} = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
