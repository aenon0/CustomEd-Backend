namespace CustomEd.Assessment.Service.DTOs
{
    public class UpdateSubmissionDto
    {
        public Guid Id { get; set; }
        public Guid StudentId { get; set; }
        public Guid AssessmentId { get; set; }
        public List<Guid> Answers { get; set; } = null!;
    }
}