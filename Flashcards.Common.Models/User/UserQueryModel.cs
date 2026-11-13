
using Flashcards.Common.Enums;

namespace Flashcards.Common.Models
{
    public class UserQueryModel : BaseQueryModel
    {
        public string? SearchString { get; set; } = null;
        public RoleType? Role { get; set; } = null;
    }
}
