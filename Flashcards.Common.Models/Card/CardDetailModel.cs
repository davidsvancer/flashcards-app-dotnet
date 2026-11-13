using Flashcards.Common.Enums;

namespace Flashcards.Common.Models
{
    public class CardDetailModel : IWithGuid
    {
        public required Guid Id { get; init; }
        public required ContentType QuestionType { get; set; }
        public required string QuestionContent { get; set; }
        public required ContentType AnswerType { get; set; }
        public required string AnswerContent { get; set; }
        public required DateTime DateCreated { get; set; }
        public UserListModel? Owner { get; set; } = null;
        public string? Description { get; set; }
    }
}
