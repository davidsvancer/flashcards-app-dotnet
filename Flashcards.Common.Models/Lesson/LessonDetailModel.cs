
namespace Flashcards.Common.Models
{
    public class LessonDetailModel : IWithGuid
    {
        public required Guid Id { get; init; }
        public required CardCollectionDetailModel CardCollection { get; set; }
        public required DateTime Start { get; set; }
        public required uint Seen { get; set; }
        public required uint Correct { get; set; }
        public required uint Wrong { get; set; }
    }
}