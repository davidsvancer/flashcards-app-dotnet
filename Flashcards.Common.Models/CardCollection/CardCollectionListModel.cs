namespace Flashcards.Common.Models
{
    public class CardCollectionListModel : IWithGuid
    {
        public required Guid Id { get; init; }
        public required string Title { get; set; }
        public required UserListModel? Owner { get; set; }
        public required int CardCount { get; set; }
        public DateTime? AnswerStart { get; set; } = null;
        public DateTime? AnswerEnd { get; set; } = null;
    }
}
