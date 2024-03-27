using CustomEd.Shared.Model;

namespace CustomEd.Notification.Service;

public class ClassroomCreatedEvent
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string CourseNo { get; set; } = null!;
    public string Description { get; set; } = null!;
    public Guid CreaterId {get; set;}
    // public Teacher Creator { get; set; } = null!;
    // public List<Student> Members {get; set;} = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
