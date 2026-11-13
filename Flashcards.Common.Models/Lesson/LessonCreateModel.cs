
namespace Flashcards.Common.Models
{
    public class LessonCreateModel
    {
        public required Guid CardCollectionId { get; set; }
        public required DateTime Start { get; set; }
        public uint Seen { get; set; } = 0;
        public uint Correct { get; set; } = 0;
        public uint Wrong { get; set; } = 0;
    }
}
