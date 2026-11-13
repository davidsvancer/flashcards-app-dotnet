using System.Security.Claims;
using Flashcards.IdentityProvider.BL.Mappers;
using Flashcards.IdentityProvider.BL.Models;
using Flashcards.IdentityProvider.DAL.Entities;
using Microsoft.AspNetCore.Identity;

namespace Flashcards.IdentityProvider.BL.Facades;

public class AppUserClaimsFacade(
    UserManager<AppUserEntity> userManager,
    AppUserClaimMapper appUserClaimMapper)
    : IAppUserClaimsFacade
{
    public async Task<IEnumerable<AppUserClaimListModel>> GetAppUserClaimsByUserIdAsync(Guid userId)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());

        if (user is null)
        {
            return new List<AppUserClaimListModel>();
        }
        else
        {
            var claims = await userManager.GetClaimsAsync(user);

            var roles = await userManager.GetRolesAsync(user);
            claims = [.. claims, .. roles.Select(role => new Claim("role", role))];

            return claims.Select(appUserClaimMapper.ToListModel);
        }
    }
}
