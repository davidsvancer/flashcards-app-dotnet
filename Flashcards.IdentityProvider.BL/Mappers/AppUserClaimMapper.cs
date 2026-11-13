using System.Security.Claims;
using Flashcards.IdentityProvider.BL.Models;

namespace Flashcards.IdentityProvider.BL.Mappers;

public class AppUserClaimMapper
{
    public AppUserClaimListModel ToListModel(Claim claim)
    { 
        return new AppUserClaimListModel
        {
            ClaimType = claim.Type,
            ClaimValue = claim.Value
        };
    }
}
