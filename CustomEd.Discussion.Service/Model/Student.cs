using CustomEd.Shared.Model;

namespace CustomEd.Discussion.Service.Model;

public class Student: BaseEntity
{
    public string StudentId { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public DateOnly DateOfBirth { get; set; }
    public Department Department { get; set; }
    public string PhoneNumber { get; set; } = null!;
    public DateOnly JoinDate { get; set; }
    public int Year { get; set; }
    public string Section { get; set; } = null!;
    public string Email { get; set; } = null!;
    public bool isDeleted {get; set;} = false;
}
