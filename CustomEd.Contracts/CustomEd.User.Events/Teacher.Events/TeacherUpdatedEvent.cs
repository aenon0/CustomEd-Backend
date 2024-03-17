using CustomEd.Shared.Model;

namespace CustomEd.User.Teacher.Events
{
    public class TeacherUpdatedEvent
    {
        public Guid Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public Department Department { get; set; }
        public string? PhoneNumber { get; set; }
        public DateOnly JoinDate { get; set; }
        public string Email { get; set; } = null!;
    }
}