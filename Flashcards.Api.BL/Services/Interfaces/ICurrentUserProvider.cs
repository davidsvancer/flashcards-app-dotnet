using Flashcards.Api.DAL.Common.Entities;

namespace Flashcards.Api.BL.Services.Interfaces
{
    public interface ICurrentUserProvider
    {
        UserEntity GetCurrentUser();
    }
}
