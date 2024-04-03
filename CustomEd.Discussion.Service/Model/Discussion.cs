using CustomEd.Shared.Model;

namespace CustomEd.Discussion.Service.Model;

public class Discussion : BaseEntity
{
    public Classroom Classroom {set; get;}
    public Student  Student {set; get;}
    public string Message {set; get;}
}
