
namespace Flashcards.Api.DAL.Common.Entities.Interfaces
{
    public interface IOwnedEntity
    {
        public Guid OwnerId { get; }
    }
}
