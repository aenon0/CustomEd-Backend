namespace CustomEd.Contracts.Classroom.Events;

public class ClassroomCreatedEvent
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public List<string>? Students { get; set; }
    public Guid CreatorId { get; set; }
    public List<Guid> MemberIds { get; set; } = new List<Guid>();
}