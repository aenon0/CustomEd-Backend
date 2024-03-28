using CustomEd.Shared.Model;

namespace CustomEd.Assessment.Service.Model
{
    public class Submission : BaseEntity
    {
        public Guid StudentId { get; set; }
        public Guid AssessmentId { get; set; }
        public List<Guid> Answers { get; set; } = null!;
    }
}
