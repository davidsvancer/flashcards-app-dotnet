using Flashcards.Common.Models;
using Flashcards.Web.BL.Api;

namespace Flashcards.Web.BL.Facades
{
    public class LessonFacade(ILessonApiClient apiClient)
    {
        public async Task <ICollection<LessonListModel>> GetAllAsync()
        {
            var lessons = await apiClient.LessonGetAsync();
            return lessons.ToList();
        }

        public async Task<Guid> Create(LessonCreateModel lesson)
        {
            return await apiClient.LessonPostAsync(lesson);
        }

        public async Task<PagedResponse<LessonListModel>> GetByQueryAsync(LessonQueryModel query)
        {
            var collection = await apiClient.QueryAsync(query);
            return collection;
        }

        public async Task<LessonDetailModel?> GetByIdAsync(Guid id)
        {
            try
            {
                var lesson = await apiClient.LessonGetAsync(id);
                return lesson;
            }
            catch
            {
                return null;
            }
        }
        
        public async Task DeleteAsync(Guid id)
        {
            await apiClient.LessonDeleteAsync(id);
        }
    }   
}
