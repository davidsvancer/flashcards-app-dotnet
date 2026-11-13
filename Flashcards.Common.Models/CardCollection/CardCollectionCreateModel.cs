
namespace Flashcards.Common.Models
{
    public class CardCollectionCreateModel
    {
        public required string Title { get; set; }
        public IList<CardCreateModel> Cards { get; set; } = new List<CardCreateModel>();
        public DateTime? AnswerStart { get; set; } = null;
        public DateTime? AnswerEnd { get; set; } = null;
    }
}
