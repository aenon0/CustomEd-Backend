namespace ISchool.Model;

public class Student
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string StudentId {set; get;}
    public string Email { get; set; }
    public string Phone { get; set; }
    public string Address { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string Country { get; set; }
    public string Zip { get; set; }
    public string Section { get; set; }
    public int Year { get; set; }
    public Department Department { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public DateOnly? JoinDate { get; set; }
}
