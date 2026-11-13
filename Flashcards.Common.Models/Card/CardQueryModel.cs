using Flashcards.Common.Enums;

namespace Flashcards.Common.Models
{
    public class CardQueryModel : BaseQueryModel
    {
        public string? SearchString { get; set; } = null;
        public ContentType? QuestionType { get; set; } = null;
        public ContentType? AnswerType { get; set; } = null;
        public Guid? OwnerId { get; set; }
    }
}
