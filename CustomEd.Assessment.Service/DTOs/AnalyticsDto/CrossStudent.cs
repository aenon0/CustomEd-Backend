namespace CustomEd.Assessment.Service.DTOs
{
    public class CrossStudent
    {
        public Guid StudentId { get; set; }
        public Guid AssessmentId { get; set; }
        public double Score { get; set; }
        public double Percentile { get; set; }
        public double StandardScore { get; set; }
    }
}