using Flashcards.Common.Enums;

namespace Flashcards.Common.Models
{
    public class UserListModel : IWithGuid
    {
        public required Guid Id { get; init; }
        public required string Name { get; set; }
        public required bool IsDeleted { get; set; }
        public required RoleType Role { get; set; }
    }
}
