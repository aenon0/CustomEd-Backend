namespace CustomEd.User.Service.Model;

public class Student: User
{
    public string? StudentId {get; set;}
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public Department Department { get; set; }
    public string? PhoneNumber {get; set;}
    public DateTime? JoinDate { get; set; }
    public int? Year { get; set; }
    public string? Section { get; set; }

}