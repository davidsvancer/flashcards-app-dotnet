using Flashcards.Api.BL.Facades;
using Flashcards.Common.Models;

namespace Flashcards.Api.BL.UnitTests
{
    public class QueryPagingTests
    {
        [Fact]
        public void PerformPagination_ShouldReturnCorrectPageSize()
        {
            // Arrange
            var data = Enumerable.Range(1, 50).Select(i => new { Id = i }).ToList();
            var query = new BaseQueryModel() { Page = 2, PageSize = 10 };

            // Act
            var result = QueryHelper.PerformPagination(data, query);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(10, result.Count);
            Assert.Equal(11, result.First().Id);// Page 2 starts at index 10 (11th item)
        }
    }
}
