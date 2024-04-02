using CustomEd.Shared.Model;

namespace CustomEd.Assessment.Service.Model
{
    public class Answer: BaseEntity
    {
        public Guid QuestionId { get; set; }
        public string Text { get; set; } = null!;
        public bool IsCorrect { get; set; }

    }
}