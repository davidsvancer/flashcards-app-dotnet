
namespace Flashcards.Common.Models
{
    public class LessonQueryModel : BaseQueryModel
    {
        public required Guid UserId { get; set; }
        public Guid? CardCollectionId { get; set; } = null;
    }
}
