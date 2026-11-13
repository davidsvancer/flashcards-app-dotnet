using Flashcards.Api.DAL.Common.Entities.Interfaces;

namespace Flashcards.Api.DAL.Common.Entities
{
    public record CardCollectionEntity : EntityBase, IOwnedEntity
    {
        public required string Title { get; set; }
        public ICollection<CardEntity> Cards { get; set; } = new List<CardEntity>();
        public DateTime? AnswerStart { get; set; } = null;
        public DateTime? AnswerEnd { get; set; } = null;
        public  Guid OwnerId { get; set; }
        public UserEntity? Owner { get; set; }
    }
}
