using Flashcards.Api.DAL.Common.Entities;
using Flashcards.Api.DAL.Common.Repositories;

namespace Flashcards.Api.DAL.IntegrationTests
{
    public interface IFakeDatabase
    {
        ICardRepository GetCardRepository();

        CardEntity GetCardById(Guid id);
        IList<Guid> CardGuids { get; }

        IUserRepository GetUserRepository();
        UserEntity? GetUserById(Guid id);
        
        IList<Guid> UserGuids { get; }

        ICardCollectionRepository GetCardCollectionRepository();
        CardCollectionEntity? GetCardCollectionById(Guid id);

        IList<Guid> CardCollectionGuids { get; }

        ILessonRepository GetLessonRepository();

        LessonEntity GetLessonById(Guid id);
        IList<Guid> LessonGuids { get; }
    }
}
