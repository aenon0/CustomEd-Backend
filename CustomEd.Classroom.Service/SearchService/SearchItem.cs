namespace CustomEd.Classroom.Service.Search.Service;

public class SearchItem
{
    public Guid Id {get; set;}
    public string Name {get; set;} = null!;
    public string Description {get; set;} = null!;
    public int Distance {get; set;}
} 