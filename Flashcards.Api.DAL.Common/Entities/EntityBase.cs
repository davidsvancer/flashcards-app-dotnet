using Flashcards.Api.DAL.Common.Entities.Interfaces;

namespace Flashcards.Api.DAL.Common.Entities
{
    public abstract record EntityBase : IEntity
    {
        public required Guid Id { get; init; }
    }
}
