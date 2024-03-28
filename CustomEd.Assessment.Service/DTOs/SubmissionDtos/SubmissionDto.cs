namespace CustomEd.Assessment.Service.DTOs
{
    public class SubmissionDto
    {

        public Guid Id { get; set; }
        public Guid StudentId { get; set; }
        public Guid AssessmentId { get; set; }
        public List<Guid> Answers { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}