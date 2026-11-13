using Flashcards.Common.BL.Facades;
using Flashcards.IdentityProvider.BL.Models;

namespace Flashcards.IdentityProvider.BL.Facades;

public interface IAppUserClaimsFacade : IFacade
{
    Task<IEnumerable<AppUserClaimListModel>> GetAppUserClaimsByUserIdAsync(Guid userId);
}
