using Flashcards.Common.BL.Facades;
using Flashcards.Common.Models;
using Flashcards.Common.Models.AppUser;
using Flashcards.IdentityProvider.BL.Models;

namespace Flashcards.IdentityProvider.BL.Facades;

public interface IAppUserFacade : IFacade
{
    Task<string?> CreateAppUserAsync(AppUserCreateModel appUserModel);
    Task<bool> DeleteAppUserAsync(string subId);
    Task<Guid?> CreateAppUserWithRoleAsync(AppUserCreateModel appUserModel, string role);
    Task<bool> ValidateCredentialsAsync(string userName, string password);
    Task<Guid> GetUserIdByUserNameAsync(string userName);
    public Task<AppUserDetailModel?> GetUserByIdAsync(Guid id);

    Task<IList<AppUserListModel>> SearchAsync(string searchString);
    Task<AppUserDetailModel?> GetUserByUserNameAsync(string userName);
    Task<AppUserDetailModel?> GetAppUserByExternalProviderAsync(string provider, string providerIdentityKey);
    Task<AppUserDetailModel> CreateExternalAppUserAsync(AppUserExternalCreateModel appUserModel);
    Task<bool> ActivateUserAsync(string securityCode, string email);
    Task<bool> IsEmailConfirmedAsync(string userName);
}
