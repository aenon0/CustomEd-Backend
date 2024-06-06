
namespace CustomEd.User.Service.Model;

public class Teacher: User
{
    public DateOnly? DateOfBirth { get; set; }
    public Department? Department { get; set; }
    public string? PhoneNumber {get; set;}
    public DateOnly? JoinDate { get; set; }

}