using Flashcards.Common.Installers;
using Flashcards.IdentityProvider.DAL.Entities;
using Flashcards.IdentityProvider.DAL.Factories;
using Flashcards.IdentityProvider.DAL.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Flashcards.IdentityProvider.DAL.Installers;

public class IdentityProviderDALInstaller : IInstaller
{
    public void Install(IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<IDbContextFactory<IdentityProviderDbContext>, IdentityProviderDbContextFactory>();

        serviceCollection.AddScoped<IUserStore<AppUserEntity>, UserStore<AppUserEntity, AppRoleEntity, IdentityProviderDbContext, Guid, AppUserClaimEntity, AppUserRoleEntity, AppUserLoginEntity, AppUserTokenEntity, AppRoleClaimEntity>>();
        serviceCollection.AddScoped<IRoleStore<AppRoleEntity>, RoleStore<AppRoleEntity, IdentityProviderDbContext, Guid, AppUserRoleEntity, AppRoleClaimEntity>>();

        serviceCollection.AddTransient<IAppUserRepository, AppUserRepository>();

        serviceCollection.AddTransient(serviceProvider =>
        {
            var dbContextFactory = serviceProvider.GetRequiredService<IDbContextFactory<IdentityProviderDbContext>>();
            return dbContextFactory.CreateDbContext();
        });
    }
}