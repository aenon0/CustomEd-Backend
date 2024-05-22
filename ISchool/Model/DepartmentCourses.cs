namespace ISchool.Model;

public class DepartmentCourses
{
    public Guid Id { get; set; }
    public Department Department {set; get;}
    public List<String> Courses {set; get;}
}
