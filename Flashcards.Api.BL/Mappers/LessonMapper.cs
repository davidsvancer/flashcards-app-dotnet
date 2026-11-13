using Flashcards.Api.DAL.Common.Entities;
using Flashcards.Common.Models;

namespace Flashcards.Api.BL.Mappers
{
    public class LessonMapper(CardCollectionMapper collectionMapper)
    {
        public LessonEntity ToEntity(LessonDetailModel model, UserEntity owner)
        {
            return new LessonEntity
            {
                Id = model.Id,
                OwnerId = owner.Id,
                User = owner,
                CardCollectionId = model.CardCollection.Id,
                CardCollection = collectionMapper.ToEntity(model.CardCollection, owner),
                Start = model.Start,
                Seen = model.Seen,
                Correct = model.Correct,
                Wrong = model.Wrong,
            };
        }

        public LessonEntity ToEntity(LessonCreateModel model, UserEntity owner, CardCollectionEntity collection)
        {
            return new LessonEntity
            {
                Id = Guid.NewGuid(),
                OwnerId = owner.Id,
                User = owner,
                CardCollectionId = model.CardCollectionId,
                CardCollection = collection,
                Start = model.Start,
                Seen = model.Seen,
                Correct = model.Correct,
                Wrong = model.Wrong,
            };
        }

        public LessonDetailModel ToDetailModel(LessonEntity entity)
        {
            return new LessonDetailModel
            {
                Id = entity.Id,
                CardCollection = collectionMapper.ToDetailModel(entity.CardCollection),
                Start = entity.Start,
                Seen = entity.Seen,
                Correct = entity.Correct,
                Wrong = entity.Wrong
            };
        }

        public List<LessonListModel> ToListModel(IEnumerable<LessonEntity> entities)
        {
            return entities.Select(e => new LessonListModel
            {
                Id = e.Id,
                CardCollectionId = e.CardCollectionId,
                CardCollectionCardCount = e.CardCollection.Cards.Count,
                CardCollectionTitle = e.CardCollection.Title,
                Start = e.Start,
                Seen = e.Seen,
                Correct = e.Correct,
                Wrong = e.Wrong
            }).ToList();
        }
    }
}
