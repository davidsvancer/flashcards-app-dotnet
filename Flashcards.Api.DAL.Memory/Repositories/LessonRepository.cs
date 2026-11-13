using Flashcards.Api.DAL.Common.Entities;
using Flashcards.Api.DAL.Common.Repositories;
using System.Linq.Expressions;
using Flashcards.Api.DAL.Common.Interfaces;

namespace Flashcards.Api.DAL.Memory.Repositories
{
    public class LessonRepository(IStorage storage) : ILessonRepository
    {
        public IList<LessonEntity> GetAll()
        {
            return storage.Lessons;
        }

        public LessonEntity? GetById(Guid id)
        {
            return storage.Lessons.SingleOrDefault(lesson => lesson.Id == id);
        }

        public LessonEntity? FirstOrDefault(Expression<Func<LessonEntity, bool>> predicate)
        {
            return storage.Lessons.AsQueryable().FirstOrDefault(predicate);
        }

        public IEnumerable<LessonEntity> Where(Expression<Func<LessonEntity, bool>> predicate)
        {
            return storage.Lessons.AsQueryable().Where(predicate);
        }

        public Guid Insert(LessonEntity entity)
        {
            storage.Lessons.Add(entity);
            return entity.Id;
        }

        public Guid? Update(LessonEntity entity)
        {
            var existingEntity = storage.Lessons.SingleOrDefault(entityInStorage => entityInStorage.Id == entity.Id);
            if (existingEntity is null)
            {
                return null;
            }

            existingEntity.CardCollection = entity.CardCollection;
            existingEntity.User = entity.User;
            existingEntity.Start = entity.Start;
            existingEntity.Seen = entity.Seen;
            existingEntity.Correct = entity.Correct;
            existingEntity.Wrong = entity.Wrong;
            return existingEntity.Id;
        }

        public void Remove(Guid id)
        {
            var entityToRemove = storage.Lessons.Single(entity => entity.Id == id);
            storage.Lessons.Remove(entityToRemove);
        }

        public bool Exists(Guid id)
        {
            return storage.Lessons.Any(entity => entity.Id == id);
        }
    }
}
