namespace CustomEd.Classroom.Service.DTOs;
public class SearchResult<T>
{
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public List<T> Results { get; set; } = null!;

}