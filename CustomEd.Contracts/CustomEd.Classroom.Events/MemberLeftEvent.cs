namespace CustomEd.Contracts.Classroom.Events
{
    public class MemberLeftEvent
    {
        public Guid ClassroomId { get; set; }
        public Guid StudentId { get; set; }
    }
}