namespace CustomEd.Discussion.Service.DTOs;

public class UpdateMessageDto
{
    public required Guid Id { get; set; }
    public required string Content { get; set; }
}
