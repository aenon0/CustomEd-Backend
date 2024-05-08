
using CustomEd.Shared.Model;

namespace CustomEd.Classroom.Service.Model;

public class Teacher : BaseEntity
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public Department Department { get; set; }
    public string? PhoneNumber {get; set;}
    public DateOnly? JoinDate { get; set; }
     public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public Role Role { get; set; }
}