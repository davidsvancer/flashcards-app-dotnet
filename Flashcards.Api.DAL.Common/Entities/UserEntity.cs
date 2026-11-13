using Flashcards.Common.Enums;

namespace Flashcards.Api.DAL.Common.Entities
{
    public record UserEntity : EntityBase
    {
        public required string Name { get; set; }
        public required RoleType Role { get; set; }
        public required string IdentityOwnerId { get; set; }
        public bool IsDeleted { get; set; } = false;
        public string? PhotoUrl { get; set; } = null;
    }
}