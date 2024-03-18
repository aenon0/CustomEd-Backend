using CustomEd.User.Service.Model;

namespace CustomEd.User.Service.DTOs;
public class TeacherDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public DateOnly DateOfBirth { get; set; }
    public string? PhoneNumber {get; set;}
    public DateOnly JoinDate { get; set; }
    public Role Role { get; set; }
    public Department Department { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
