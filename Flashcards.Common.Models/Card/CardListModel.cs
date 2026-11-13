using Flashcards.Common.Enums;

namespace Flashcards.Common.Models
{
    public class CardListModel : IWithGuid
    {
        public required Guid Id { get; init; }
        public required ContentType QuestionType { get; set; }
        public required string QuestionContent { get; set; }
        public required ContentType AnswerType { get; set; }
        public string? OwnerName { get; set; } = null;
        public string? Description { get; set; } = null;
    }
}
