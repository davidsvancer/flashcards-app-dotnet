using Flashcards.IdentityProvider.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace Flashcards.IdentityProvider.DAL.Repositories
{
    public class AppUserRepository(IdentityProviderDbContext identityProviderDbContext) : IAppUserRepository
    {
        public async Task<IList<AppUserEntity>> SearchAsync(string searchString)
        {
            return await identityProviderDbContext.Users
                .Where(user => EF.Functions.Like(user.UserName, $"%{searchString}"))
                .ToListAsync();
        }
    }
}
