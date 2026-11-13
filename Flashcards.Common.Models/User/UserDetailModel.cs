using Flashcards.Common.Enums;

namespace Flashcards.Common.Models
{
    public class UserDetailModel : IWithGuid
    {
        public required Guid Id { get; init; }
        public required string Name { get; set; }
        public required RoleType Role { get; set; }
        public bool IsDeleted { get; set; }
        public string? PhotoUrl { get; set; } = null;
    }
}

