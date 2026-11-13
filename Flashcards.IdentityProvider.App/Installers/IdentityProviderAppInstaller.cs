using Flashcards.Common.Installers;
using Flashcards.IdentityProvider.DAL;
using Flashcards.IdentityProvider.DAL.Entities;
using Microsoft.AspNetCore.Identity;

namespace Flashcards.IdentityProvider.App.Installers;

public class IdentityProviderAppInstaller : IInstaller
{
    public void Install(IServiceCollection serviceCollection)
    {
        serviceCollection.AddIdentity<AppUserEntity, AppRoleEntity>()
            .AddEntityFrameworkStores<IdentityProviderDbContext>()
            .AddTokenProvider<DataProtectorTokenProvider<AppUserEntity>>(TokenOptions.DefaultProvider);

        serviceCollection.Configure<IdentityOptions>(options =>
        {
            options.Password.RequireDigit = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredLength = 6;

            options.SignIn.RequireConfirmedEmail = true;
        });
    }
}
