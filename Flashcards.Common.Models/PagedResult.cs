
namespace Flashcards.Common.Models
{
    public class PagedResponse<T>(IList<T> items, int totalCount, int pageNumber, int pageSize)
    {
        public IList<T> Items { get; set; } = items;

        // Metadata for the UI to build the pager
        public int TotalCount { get; set; } = totalCount;
        public int PageNumber { get; set; } = pageNumber;
        public int PageSize { get; set; } = pageSize;

        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    }
}