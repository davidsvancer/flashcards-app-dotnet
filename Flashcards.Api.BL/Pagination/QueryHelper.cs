
using Flashcards.Common.Models;
using System.Reflection;

namespace Flashcards.Api.BL.Facades
{
    public static class QueryHelper
    {
        public static IList<TEntity> PerformPagination<TEntity>(IEnumerable<TEntity> collection, BaseQueryModel query)
        {
            var source = collection.AsQueryable();

            // 1. Handle Dynamic Sorting via Reflection
            if (!string.IsNullOrWhiteSpace(query.SortBy))
            {
                // Find the property on the entity (case-insensitive)
                var prop = typeof(TEntity).GetProperty(query.SortBy,
                    BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                if (prop != null)
                {
                    source = query.SortAsc
                        ? source.OrderBy(x => prop.GetValue(x, null))
                        : source.OrderByDescending(x => prop.GetValue(x, null));
                }
            }

            // 2. Perform Paging
            // Formula: (Page - 1) * PageSize
            return source
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToList();
        }
    }
}
