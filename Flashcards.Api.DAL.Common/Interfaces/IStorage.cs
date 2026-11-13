using Flashcards.Api.DAL.Common.Entities;

namespace Flashcards.Api.DAL.Common.Interfaces
{
    public interface IStorage
    {
        public IList<CardEntity> Cards { get; }
        public IList<UserEntity> Users { get; }
        public IList<CardCollectionEntity> Collections { get; }
        public IList<LessonEntity> Lessons { get; }
        public void Seed();
    }
}
