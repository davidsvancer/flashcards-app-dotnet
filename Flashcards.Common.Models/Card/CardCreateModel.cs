using Flashcards.Common.Enums;

namespace Flashcards.Common.Models
{
    public class CardCreateModel
    {
        public required ContentType QuestionType { get; set; }
        public required string QuestionContent { get; set; }
        public required ContentType AnswerType { get; set; }
        public required string AnswerContent { get; set; }
        public string? Description { get; set; } = null;
    }
}
