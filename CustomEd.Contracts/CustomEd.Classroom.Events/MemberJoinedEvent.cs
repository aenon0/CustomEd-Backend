namespace CustomEd.Contracts.Classroom.Events;

public class MemberJoinedEvent
{
    public Guid StudentId { get; set; }
    public Guid ClassroomId{get; set;}

}