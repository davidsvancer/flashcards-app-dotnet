namespace Flashcards.Common.Models
{
    public class LessonListModel : IWithGuid
    {
        public required Guid Id { get; init; }
        public required Guid CardCollectionId { get; set; }
        public required int CardCollectionCardCount { get; set; }
        public required string CardCollectionTitle { get; set; }
        public required DateTime Start { get; set; }
        public required uint Seen { get; set; }
        public required uint Correct { get; set; }
        public required uint Wrong { get; set; }
    }
}
