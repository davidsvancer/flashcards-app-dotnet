namespace Flashcards.Common.Models
{
    public class BaseQueryModel
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string? SortBy { get; set; } = null;
        public bool SortAsc { get; set; } = true;
    }
}