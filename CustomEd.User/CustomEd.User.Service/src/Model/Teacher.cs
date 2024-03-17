
namespace CustomEd.User.Service.Model;

public class Teacher: User
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public Department Department { get; set; }
    public string? PhoneNumber {get; set;}
    public DateTime? JoinDate { get; set; }

}