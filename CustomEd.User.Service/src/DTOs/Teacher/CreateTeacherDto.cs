using CustomEd.User.Service.Model;

namespace CustomEd.User.Service.DTOs
{
    public class CreateTeacherDto
    {
        // public string? FirstName { get; set; }
        // public string? LastName { get; set; }
        // public DateOnly DateOfBirth { get; set; }
        // public Department Department { get; set; }
        // public string? PhoneNumber { get; set; }
        // public DateOnly JoinDate { get; set; }
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
