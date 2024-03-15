using CustomEd.Shared.Model;

namespace CustomEd.User.Student.Events
{
    public class StudentUpdatedEvent
    {
        public Guid Id { get; set; }
        public string StudentId { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public DateTime DateOfBirth { get; set; }
        public Department Department { get; set; }
        public string PhoneNumber { get; set; } = null!;
        public DateTime JoinDate { get; set; }
        public int Year { get; set; }
        public string Section { get; set; } = null!;
        public string Email { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt {get; set;} 
    }
}