using CustomEd.Assessment.Service.DTOs;
using CustomEd.Assessment.Service.Model;
using CustomEd.Shared.Data.Interfaces;
using MathNet.Numerics;
using MathNet.Numerics.Statistics;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace CustomEd.Assessment.Service.AnalyticsSevice
{
    public class AnalysisService
    {
        private readonly IGenericRepository<Model.Assessment> _assessmentRepository;
        private readonly IGenericRepository<Answer> _answerRepository;
        private readonly IGenericRepository<Submission> _submissionRepository;
        private readonly IGenericRepository<Question> _questionRepository;
        private readonly IGenericRepository<Analytics> _analyticsRepository;
        private readonly Dictionary<string, double> _scorePerTopic;

        public AnalysisService(
            IGenericRepository<Model.Assessment> assessmentRepository,
            IGenericRepository<Answer> answerRepository,
            IGenericRepository<Submission> submissionRepository,
            IGenericRepository<Question> questionRepository,
            IGenericRepository<Analytics> analyticsRepository
        )
        {
            _assessmentRepository = assessmentRepository;
            _answerRepository = answerRepository;
            _submissionRepository = submissionRepository;
            _questionRepository = questionRepository;
            _analyticsRepository = analyticsRepository;
            _scorePerTopic = new Dictionary<string, double>();
        }

        public async Task<Analytics?> PerformClassAnalysis(Guid assessmentId)
        {
            var analytics = await _analyticsRepository.GetAsync(x => x.Assessment.Id == assessmentId);
            var assessment = await _assessmentRepository.GetAsync(assessmentId);
            if (assessment == null)
            {
                return null;
            }
            
            if(assessment.Deadline > DateTime.UtcNow && analytics != null)
            {
                return analytics;
            }

            var submissions = await _submissionRepository.GetAllAsync(s =>
                s.AssessmentId == assessmentId
            );
            var questions = await _questionRepository.GetAllAsync(q =>
                q.AssessmentId == assessmentId
            );

            var totalScore = 0.0;
            var totalQuestions = questions.Count();
            var totalSubmissions = submissions.Count();

            var scores = new List<double>();
            foreach (var submission in submissions)
            {
                var score = 0.0;
                var answers = await _answerRepository.GetAllAsync(a =>
                    submission.Answers.Contains(a.Id)
                );
                foreach (var answer in answers)
                {
                    var question = questions.FirstOrDefault(q => q.Id == answer.QuestionId);
                    if (question != null)
                    {
                        if (answer.IsCorrect)
                        {
                            score += question.Weight;
                            foreach (var topic in question.Tags)
                            {
                                if (_scorePerTopic.ContainsKey(topic))
                                {
                                    _scorePerTopic[topic] += question.Weight;
                                }
                                else
                                {
                                    _scorePerTopic.Add(topic, question.Weight);
                                }
                            }
                        }
                    }
                }
                scores.Add(score);
            }

            totalScore = scores.Sum();
            var averageScore = totalScore / totalSubmissions;
            var medianScore = GetMedian(scores);
            var modeScore = GetMode(scores);
            var standardDeviation = GetStandardDeviation(scores);
            var variance = GetVariance(scores);
            var interquartileRange = GetInterquartileRange(scores);
            var skewness = GetSkewness(scores);
            var kurtosis = GetKurtosis(scores);
            var coefficientOfVariation = GetCoefficientOfVariation(scores);
            var meanAbsoluteDeviation = GetMeanAbsoluteDeviation(scores);
            var medianAbsoluteDeviation = GetMedianAbsoluteDeviation(scores);
            var modeAbsoluteDeviation = GetModeAbsoluteDeviation(scores);

            foreach (var topic in _scorePerTopic.Keys.ToList())
            {
                _scorePerTopic[topic] /= totalSubmissions;
            }


            var newAnalytics =  new Analytics
            {
                TotalScore = totalScore,
                TotalQuestions = totalQuestions,
                TotalSubmissions = totalSubmissions,
                MeanScore = averageScore,
                MedianScore = medianScore,
                ModeScore = modeScore,
                StandardDeviation = standardDeviation,
                Variance = variance,
                InterquartileRange = interquartileRange,
                Skewness = skewness,
                Kurtosis = kurtosis,
                CoefficientOfVariation = coefficientOfVariation,
                MeanAbsoluteDeviation = meanAbsoluteDeviation,
                MedianAbsoluteDeviation = medianAbsoluteDeviation,
                ModeAbsoluteDeviation = modeAbsoluteDeviation,
                TopFiveScores = scores.OrderByDescending(x => x).Take(5).ToList(),
                BottomFiveScores = scores.OrderBy(x => x).Take(5).ToList(),
                Range = scores.Max() - scores.Min(),
                MeanScorePerTopic = _scorePerTopic
            };
        
            if (analytics == null)
            {
                await _analyticsRepository.CreateAsync(newAnalytics);
            }
            else
            {
                newAnalytics.Id = analytics.Id;
                await _analyticsRepository.UpdateAsync(newAnalytics);
            }
        
            return newAnalytics;
        }

        public async Task<List<Analytics?>> PerformCrossAssessment(Guid classroomId)
        {
            var assessments = await _assessmentRepository.GetAllAsync(a =>
                a.Classroom.Id == classroomId
            );
            var listOfanalytics = new List<Analytics?>();

            foreach (var assessment in assessments)
            {
                var analytics = await PerformClassAnalysis(assessment.Id);
                listOfanalytics.Add(analytics);
            }
            return listOfanalytics;
            
        }

        public async Task<List<CrossStudent?>> PerformCrossStudent(Guid StudentId, Guid classRoomId)
        {
            var submissions = await _submissionRepository.GetAllAsync(s => s.StudentId == StudentId);
            var assessments = await _assessmentRepository.GetAllAsync(a => a.Classroom.Id == classRoomId);
        

            var crossStudents = new List<CrossStudent?>();
            foreach (var assessment in assessments)
            {
                var submission = submissions.FirstOrDefault(s => s.AssessmentId == assessment.Id);
                var allSubmissions = await _submissionRepository.GetAllAsync(s => s.AssessmentId == assessment.Id);
                var allScores = new List<double>();

                foreach (var sub in allSubmissions)
                {
                    var answers = await _answerRepository.GetAllAsync(a =>
                        sub.Answers.Contains(a.Id)
                    );
                    var questions = await _questionRepository.GetAllAsync(q =>
                        q.AssessmentId == assessment.Id
                    );

                    var totalScore = 0.0;
                    var totalQuestions = questions.Count();
                    var scores = new List<double>();
                    foreach (var answer in answers)
                    {
                        var question = questions.FirstOrDefault(q => q.Id == answer.QuestionId);
                        if (question != null)
                        {
                            if (answer.IsCorrect)
                            {
                                totalScore += question.Weight;
                                scores.Add(question.Weight);
                            }
                        }
                    }
                    allScores.Add(totalScore);
                }

                if (submission != null)
                {
                    var answers = await _answerRepository.GetAllAsync(a =>
                        submission.Answers.Contains(a.Id)
                    );
                    var questions = await _questionRepository.GetAllAsync(q =>
                        q.AssessmentId == assessment.Id
                    );

                    var totalScore = 0.0;
                    var totalQuestions = questions.Count();
                    var scores = new List<double>();
                    foreach (var answer in answers)
                    {
                        var question = questions.FirstOrDefault(q => q.Id == answer.QuestionId);
                        if (question != null)
                        {
                            if (answer.IsCorrect)
                            {
                                totalScore += question.Weight;
                                scores.Add(question.Weight);
                            }
                        }
                    }

                    var percentile = GetPercentile(allScores, totalScore);
                    var standardScore = GetStandardScore(allScores, totalScore);

                    var crossStudent = new CrossStudent
                    {
                        StudentId = StudentId,
                        AssessmentId = assessment.Id,
                        Score = totalScore,
                        Percentile = percentile,
                        StandardScore = standardScore
                    };
                    crossStudents.Add(crossStudent);
                }
            }
            return crossStudents;
        }

        public async Task<List<Analytics?>> PerformClassAnalysisByTag(List<string> tags, Guid classRoomId)
        {
            var assessments = await _assessmentRepository.GetAllAsync(a =>
                tags.Any(tag => a.Tag.Contains(tag) && a.Classroom.Id == classRoomId)
            );

            var listOfanalytics = new List<Analytics>();

            foreach (var assessment in assessments)
            {
                var analytics = await PerformClassAnalysis(assessment.Id);
                listOfanalytics.Add(analytics);
            }
            return listOfanalytics;
        }
        private double GetPercentile(List<double> scores, double totalScore)
        {
            scores.Sort();
            int N = scores.Count;
            int n = scores.IndexOf(totalScore) + 1;
            double percentile = (double)n / N;
            return percentile * 100;
        }
        
        private double GetStandardScore(List<double> scores, double score)
        {
            double average = scores.Average();
            double standardDeviation = GetStandardDeviation(scores);
            return (score - average) / standardDeviation;
        }
        
        private double GetMode(List<double> scores)
        {
            return scores
                .GroupBy(i => i)
                .OrderByDescending(grp => grp.Count())
                .Select(grp => grp.Key)
                .First();
        }

        private double GetStandardDeviation(List<double> scores)
        {
            double average = scores.Average();
            double sumOfSquaresOfDifferences = scores
                .Select(val => (val - average) * (val - average))
                .Sum();
            double standardDeviation = Math.Sqrt(sumOfSquaresOfDifferences / scores.Count);
            return standardDeviation;
        }

        private double GetVariance(List<double> scores)
        {
            double average = scores.Average();
            double sumOfSquaresOfDifferences = scores
                .Select(val => (val - average) * (val - average))
                .Sum();
            double variance = sumOfSquaresOfDifferences / scores.Count;
            return variance;
        }

        private double GetInterquartileRange(List<double> scores)
        {
            var sortedScores = scores.OrderBy(x => x).ToList();
            double lowerQuartile = sortedScores[sortedScores.Count / 4];
            double upperQuartile = sortedScores[sortedScores.Count * 3 / 4];
            return upperQuartile - lowerQuartile;
        }


        private double GetSkewness(List<double> scores)
        {
            if (scores.Count < 3)
            {
                throw new ArgumentException("At least 3 scores are required to calculate skewness.");
            }

            double mean = Statistics.Mean(scores);
            double variance = Statistics.Variance(scores);
            double stdDev = Math.Sqrt(variance);
            
            double skewness = scores.Select(x => Math.Pow((x - mean) / stdDev, 3)).Sum() /
                            (scores.Count * Math.Pow(stdDev, 3));
            
            return skewness;
        }

        private double GetKurtosis(List<double> scores)
        {
            if (scores.Count < 4)
            {
                throw new ArgumentException("At least 4 scores are required to calculate kurtosis.");
            }

            double mean = Statistics.Mean(scores);
            double variance = Statistics.Variance(scores);
            double stdDev = Math.Sqrt(variance);
            
            double kurtosis = scores.Select(x => Math.Pow((x - mean) / stdDev, 4)).Sum() /
                            (scores.Count * Math.Pow(variance, 2)) - 3;
            
            return kurtosis;
        }


        private double GetCoefficientOfVariation(List<double> scores)
        {
            double standardDeviation = GetStandardDeviation(scores);
            double mean = scores.Average();
            return standardDeviation / mean;
        }

        private double GetMeanAbsoluteDeviation(List<double> scores)
        {
            double mean = scores.Average();
            double sumOfAbsoluteDifferences = scores.Select(val => Math.Abs(val - mean)).Sum();
            return sumOfAbsoluteDifferences / scores.Count;
        }

        private double GetMedianAbsoluteDeviation(List<double> scores)
        {
            double median = GetMedian(scores);
            double sumOfAbsoluteDifferences = scores.Select(val => Math.Abs(val - median)).Sum();
            return sumOfAbsoluteDifferences / scores.Count;

        }

        private double GetModeAbsoluteDeviation(List<double> scores)
        {
            double mode = GetMode(scores);
            double sumOfAbsoluteDifferences = scores.Select(val => Math.Abs(val - mode)).Sum();
            return sumOfAbsoluteDifferences / scores.Count;
        }

        private double GetMedian(List<double> scores)
        {
            int count = scores.Count;
            if (count == 0)
            {
                throw new InvalidOperationException("Empty collection");
            }
            scores.Sort();
            if (count % 2 == 0)
            {
                return (scores[count / 2 - 1] + scores[count / 2]) / 2;
            }
            else
            {
                return scores[count / 2];
            }
        }
    }
}
