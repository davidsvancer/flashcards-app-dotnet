
namespace Flashcards.Common.Models
{
    public class AppUserCreateModel
    {
        public required string UserName { get; set; }
        public required string Password { get; set; }
        public required string Subject { get; set; }
        public required string Email { get; set; }
        public string? Role { get; set; }
    }

}
