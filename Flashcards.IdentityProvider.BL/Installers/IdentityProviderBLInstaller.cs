using Flashcards.Common.BL.Facades;
using Flashcards.Common.Installers;
using Flashcards.IdentityProvider.BL.Facades;
using Flashcards.IdentityProvider.BL.Mappers;
using Microsoft.Extensions.DependencyInjection;

namespace Flashcards.IdentityProvider.BL.Installers;

public class IdentityProviderBLInstaller : IInstaller
{
    public void Install(IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<AppUserClaimMapper>();
        serviceCollection.AddSingleton<AppUserMapper>();

        serviceCollection.AddScoped<IAppUserFacade, AppUserFacade>();
        serviceCollection.AddScoped<IAppUserClaimsFacade, AppUserClaimsFacade>();
    }
}
