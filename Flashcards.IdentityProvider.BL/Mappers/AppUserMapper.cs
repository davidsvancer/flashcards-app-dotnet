using Flashcards.Common.Models;
using Flashcards.Common.Models.AppUser;
using Flashcards.IdentityProvider.BL.Models;
using Flashcards.IdentityProvider.DAL.Entities;

namespace Flashcards.IdentityProvider.BL.Mappers;

public partial class AppUserMapper
{
    public AppUserEntity ToEntity(AppUserCreateModel appUserCreateModel)
    {
        if (appUserCreateModel is null) throw new ArgumentNullException(nameof(appUserCreateModel));

        return new AppUserEntity
        {
            Id = Guid.NewGuid(),
            UserName = appUserCreateModel.UserName,
            NormalizedUserName = appUserCreateModel.UserName?.ToUpperInvariant(),
            Email = appUserCreateModel.Email,
            NormalizedEmail = appUserCreateModel.Email?.ToUpperInvariant(),
            Subject = appUserCreateModel.Subject,
            Active = true,
            SecurityStamp = Guid.NewGuid().ToString()
        };
    }

    public AppUserListModel ToListModel(AppUserEntity appUserEntity)
    {
        if (appUserEntity is null) throw new ArgumentNullException(nameof(appUserEntity));

        return new AppUserListModel
        {
            Id = appUserEntity.Id,
            UserName = appUserEntity.UserName
        };
    }

    public List<AppUserListModel> ToListModels(IEnumerable<AppUserEntity> appUserEntities)
    {
        if (appUserEntities is null) return new List<AppUserListModel>();

        return appUserEntities.Select(ToListModel).ToList();
    }

    public AppUserDetailModel ToDetailModel(AppUserEntity appUserEntity)
    {
        if (appUserEntity is null) throw new ArgumentNullException(nameof(appUserEntity));

        return new AppUserDetailModel
        {
            Id = appUserEntity.Id,
            Subject = appUserEntity.Subject,
            UserName = appUserEntity.UserName
        };
    }
}
