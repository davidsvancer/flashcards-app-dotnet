using Flashcards.Api.DAL.Common.Entities.Interfaces;

namespace Flashcards.Api.DAL.Common.Entities
{
    public record LessonEntity : EntityBase, IOwnedEntity
    {
        public Guid OwnerId { get; set; }
        public required UserEntity User { get; set; }
        public Guid CardCollectionId { get; set; }
        public required CardCollectionEntity CardCollection { get; set; }
        public required DateTime Start { get; set; }
        public required uint Seen { get; set; }
        public required uint Correct { get; set; }
        public required uint Wrong { get; set; }
    }
}