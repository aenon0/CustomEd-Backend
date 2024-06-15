using CustomEd.Shared.Model;

namespace CustomEd.Assessment.Service.Model;

public class Analytics : BaseEntity
{
    public string? Description { get; set; }
    public Assessment Assessment { get; set; } = null!;
    public double TotalScore { get; set; }
    public double TotalQuestions { get; set; }
    public double TotalSubmissions { get; set; }
    public double MeanScore { get; set; }
    public double MedianScore { get; set; }
    public double ModeScore { get; set; }
    public double StandardDeviation { get; set; }
    public double Variance { get; set; }
    public double HighestScore { get; set; }
    public double LowestScore { get; set; }
    public double Range { get; set; }
    public double InterquartileRange { get; set; }
    public double Skewness { get; set; }
    public double Kurtosis { get; set; }
    public double CoefficientOfVariation { get; set; }
    public double MeanAbsoluteDeviation { get; set; }
    public double MedianAbsoluteDeviation { get; set; }
    public double ModeAbsoluteDeviation { get; set; }
    public List<double> TopFiveScores { get; set; } = null!;
    public List<double> BottomFiveScores { get; set; } = null!;
    public Dictionary<string, double>? MeanScorePerTopic { get; set; }
}
