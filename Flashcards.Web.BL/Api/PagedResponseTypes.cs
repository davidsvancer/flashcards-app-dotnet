using Flashcards.Common.Models;

namespace Flashcards.Web.BL.Api
{
    // Trick to make Generated API client compatible with PagedResponse<T> because
    // NSWAG Generator CAN NOT handle generic types.
    public class PagedResponseOfLessonListModel(IList<LessonListModel> items, int totalCount, int pageNumber, int pageSize) 
        : PagedResponse<LessonListModel>(items, totalCount, pageNumber, pageSize) { }
    public class PagedResponseOfUserListModel(IList<UserListModel> items, int totalCount, int pageNumber, int pageSize) 
        : PagedResponse<UserListModel>(items, totalCount, pageNumber, pageSize) { }
    public class PagedResponseOfCardListModel(IList<CardListModel> items, int totalCount, int pageNumber, int pageSize)
        : PagedResponse<CardListModel>(items, totalCount, pageNumber, pageSize) { }
    public class PagedResponseOfCardCollectionListModel(IList<CardCollectionListModel> items, int totalCount, int pageNumber, int pageSize)
        : PagedResponse<CardCollectionListModel>(items, totalCount, pageNumber, pageSize)
    { }
}
