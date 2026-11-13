
namespace Flashcards.Common.Models
{
    public class CardCollectionQueryModel : BaseQueryModel
    {
        public string? SearchString { get; set; }
        public Guid? OwnerId { get; set; }
        public Guid? ExcludeOwnerId { get; set; }
        public bool ShowCurrentlyAvailableOnly { get; set; } = false;
    }
}
