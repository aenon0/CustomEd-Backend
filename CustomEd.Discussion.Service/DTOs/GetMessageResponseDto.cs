namespace CustomEd.Discussion.Service.DTOs;

public class GetMessageResponseDto
{
    public Guid? Id {get; set;}
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string Content {set; get;}
    public DateTime SentAt { get; set; }
}
