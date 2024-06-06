namespace CustomEd.User.Service.Model;

public class Student: User
{
    public string? StudentId {get; set;}
    public DateOnly? DateOfBirth { get; set; }
    public Department? Department { get; set; }
    public string? PhoneNumber {get; set;}
    public DateOnly? JoinDate { get; set; }
    public int? Year { get; set; }
    public string? Section { get; set; }

}