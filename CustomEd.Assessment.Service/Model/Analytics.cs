using CustomEd.Shared.Model;

namespace CustomEd.Assessment.Service.Model;

public class Analytics : BaseEntity
{
    public string Description { get; set; } = null!;
    public Guid AssessmentId { get; set; }
    public double MeanScore { get; set; }
    public List<double> TopFiveScores { get; set; } = null!;
    public List<double> BottomFiveScores { get; set; } = null!;
    public Dictionary<string, double> MeanScorePerTopic { get; set; } = null!;
}
