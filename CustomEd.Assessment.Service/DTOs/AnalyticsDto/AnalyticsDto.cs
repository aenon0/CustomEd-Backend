namespace CustomEd.Assessment.Service.DTOs;

public class AnalyticsDto
{
    public Guid Id { get; set; }
    public string? Description { get; set; }
    public AssessmentDto Assessment { get; set; } = null!;
    public double MeanScore { get; set; }
    public List<double> TopFiveScores { get; set; } = null!;
    public List<double> BottomFiveScores { get; set; } = null!;
    public Dictionary<string, double> MeanScorePerTopic { get; set; } = null!;
}
