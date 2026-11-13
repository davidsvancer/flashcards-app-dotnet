namespace Flashcards.Common.Models
{
    public class CardCollectionDetailModel : IWithGuid
    {
        public required Guid Id { get; init; }
        public required string Title { get; set; }
        public IList<CardDetailModel> Cards { get; set; } = new List<CardDetailModel>();
        public DateTime? AnswerStart { get; set; } = null;
        public DateTime? AnswerEnd { get; set; } = null;
        public UserDetailModel? Owner { get; set; } = null;
    }
}
