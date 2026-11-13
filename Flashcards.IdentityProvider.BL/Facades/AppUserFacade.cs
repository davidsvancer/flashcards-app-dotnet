using Flashcards.Common.Models;
using Flashcards.Common.Models.AppUser;
using Flashcards.IdentityProvider.BL.Mappers;
using Flashcards.IdentityProvider.BL.Models;
using Flashcards.IdentityProvider.DAL.Entities;
using Flashcards.IdentityProvider.DAL.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Flashcards.IdentityProvider.BL.Facades;

public class AppUserFacade(
    UserManager<AppUserEntity> userManager,
    IAppUserRepository appUserRepository,
    AppUserMapper appUserMapper) : IAppUserFacade
{
    public async Task<string?> CreateAppUserAsync(AppUserCreateModel appUserModel)
    {
        if (await userManager.FindByNameAsync(appUserModel.UserName) is not null)
        {
            throw new ArgumentException($"User with username '{appUserModel.UserName}' already exists");
        }

        var appUserEntity = appUserMapper.ToEntity(appUserModel);
        var result = await userManager.CreateAsync(appUserEntity, appUserModel.Password);
        
        if (!result.Succeeded)
        {
            throw new InvalidOperationException($"Failed to create user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }

        var createdUser = await userManager.FindByNameAsync(appUserModel.UserName);
        
        // Assign role if provided
        if (createdUser is not null && !string.IsNullOrWhiteSpace(appUserModel.Role))
        {
            var roleResult = await userManager.AddToRoleAsync(createdUser, appUserModel.Role);
            if (!roleResult.Succeeded)
            {
                Console.Write($"Role '{appUserModel.Role}' assignment to user '{appUserModel.UserName}' failed. ");
            }
        }

        return createdUser?.Subject;
    }

    public async Task<bool> DeleteAppUserAsync(string subId)
    {
        // Find user by given Subject
        var user = await userManager.Users.FirstOrDefaultAsync(u => u.Subject == subId);
        if (user is null)
        {
            return false;
        }

        // Delete found users identity
        var result = await userManager.DeleteAsync(user);
        return result.Succeeded;
    }

    public async Task<Guid?> CreateAppUserWithRoleAsync(AppUserCreateModel appUserModel, string role)
    {
        if (await userManager.FindByNameAsync(appUserModel.UserName) is not null)
        {
            throw new ArgumentException($"User with username '{appUserModel.UserName}' already exists");
        }

        var appUserEntity = appUserMapper.ToEntity(appUserModel);
        var result = await userManager.CreateAsync(appUserEntity, appUserModel.Password);
        
        if (!result.Succeeded)
        {
            throw new InvalidOperationException($"Failed to create user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }

        var createdUser = await userManager.FindByNameAsync(appUserModel.UserName);
        
        if (createdUser is not null && !string.IsNullOrWhiteSpace(role))
        {
            var roleResult = await userManager.AddToRoleAsync(createdUser, role);
            if (!roleResult.Succeeded)
            {
                throw new InvalidOperationException($"Failed to assign role: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
            }
        }

        return createdUser?.Id;
    }

    public async Task<bool> ValidateCredentialsAsync(string userName, string password)
    {
        var user = await userManager.FindByNameAsync(userName);
        if (user is null)
        {
            return false;
        }
        else
        {
            return await userManager.CheckPasswordAsync(user, password);
        }
    }

    public async Task<Guid> GetUserIdByUserNameAsync(string userName)
    {
        var user = await userManager.FindByNameAsync(userName);

        if (user is null)
        {
            throw new Exception("User not found");
        }

        return user.Id;
    }

    public async Task<AppUserDetailModel?> GetUserByIdAsync(Guid id)
    {
        var user = await userManager.FindByIdAsync(id.ToString());

        if(user is null)
        {
            return null;
        }

        return appUserMapper.ToDetailModel(user);
    }

    public async Task<AppUserDetailModel?> GetUserByUserNameAsync(string userName)
    {
        var user = await userManager.FindByNameAsync(userName);

        if (user is null)
        {
            return null;
        }

        return appUserMapper.ToDetailModel(user);
    }

    public async Task<AppUserDetailModel?> GetAppUserByExternalProviderAsync(string provider, string providerIdentityKey)
    {
        var user = await userManager.FindByLoginAsync(provider, providerIdentityKey);
        return user is not null
            ? appUserMapper.ToDetailModel(user)
            : null;
    }

    public async Task<AppUserDetailModel?> CreateExternalAppUserAsync(AppUserExternalCreateModel appUserModel)
    {
        var appUserEntity = new AppUserEntity
        {
            Id = Guid.NewGuid(),
            Active = true,
            Subject = Guid.NewGuid().ToString(),
            Email = appUserModel.Email,
        };

        var userLoginInfo = new UserLoginInfo(appUserModel.Provider, appUserModel.ProviderIdentityKey, null);

        await userManager.CreateAsync(appUserEntity);
        await userManager.AddLoginAsync(appUserEntity, userLoginInfo);

        var createdAppUserEntity = await userManager.FindByNameAsync(appUserEntity.UserName);

        return createdAppUserEntity is not null
            ? appUserMapper.ToDetailModel(createdAppUserEntity)
            : null;
    }

    public async Task<bool> ActivateUserAsync(string securityCode, string email)
    {
        var appUserEntity = await userManager.FindByEmailAsync(email);
        var identityResult = await userManager.ConfirmEmailAsync(appUserEntity, securityCode);

        return identityResult.Succeeded;
    }

    public async Task<bool> IsEmailConfirmedAsync(string userName)
    {
        var user = await userManager.FindByNameAsync(userName);
        if (user is null)
        {
            return false;
        }
        else
        {
            return await userManager.IsEmailConfirmedAsync(user);
        }
    }

    public async Task<IList<AppUserListModel>> SearchAsync(string searchString)
    {
        var users = await appUserRepository.SearchAsync(searchString);
        return appUserMapper.ToListModels(users);
    }
}
