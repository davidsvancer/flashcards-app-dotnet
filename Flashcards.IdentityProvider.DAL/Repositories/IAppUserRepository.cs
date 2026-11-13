using Flashcards.IdentityProvider.DAL.Entities;

namespace Flashcards.IdentityProvider.DAL.Repositories
{
    public interface IAppUserRepository
    {
        Task<IList<AppUserEntity>> SearchAsync(string searchString);
    }
}
