using Flashcards.Api.BL.Mappers;
using Flashcards.Api.BL.Services.Interfaces;
using Flashcards.Api.DAL.Common.Repositories;
using Flashcards.Common.Models;

namespace Flashcards.Api.BL.Facades
{
    public class LessonFacade(
        LessonMapper mapper, 
        ILessonRepository lessonRepository, 
        ICurrentUserProvider currentUser, 
        ICardCollectionRepository cardCollectionRepository) :
        ILessonFacade
    {
        public Guid Create(LessonCreateModel model)
        {
            var requestorUserEntity = currentUser.GetCurrentUser();

            var collection = cardCollectionRepository.GetById(model.CardCollectionId);
            if(collection is null)
                throw new ArgumentException("Existing collection ID is required to create a lesson.");

            var entity = mapper.ToEntity(model, requestorUserEntity, collection);
            return lessonRepository.Insert(entity);
        }

        public LessonDetailModel? GetById(Guid id)
        {
            var result = lessonRepository.GetById(id);
            if (result == null) return null;
            return mapper.ToDetailModel(result);
        }

        public List<LessonListModel> GetAll()
        {
            var entities = lessonRepository.GetAll();
            return mapper.ToListModel(entities);
        }

        public void Delete(Guid id)
        {
            lessonRepository.Remove(id);
        }

        public PagedResponse<LessonListModel> GetByQuery(LessonQueryModel query)
        {
            var collection = lessonRepository.GetAll().AsEnumerable();

            collection = collection.Where(lesson => lesson.User.Id == query.UserId);

            if (query.CardCollectionId is not null)
            {
                collection = collection.Where(lesson => lesson.CardCollection.Id == query.CardCollectionId);
            }

            var totalCount = collection.Count();
            collection = QueryHelper.PerformPagination(collection, query);
            var result = mapper.ToListModel(collection.ToList());

            return new PagedResponse<LessonListModel>(result, totalCount, query.Page, query.PageSize);
        }
    }
}
