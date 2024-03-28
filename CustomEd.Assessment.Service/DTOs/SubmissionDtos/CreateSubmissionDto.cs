namespace CustomEd.Assessment.Service.DTOs
{
    public class CreateSubmissionDto
    {
        public Guid StudentId { get; set; }
        public Guid AssessmentId { get; set; }
        public List<Guid> Answers { get; set; } = null!;
    }
}