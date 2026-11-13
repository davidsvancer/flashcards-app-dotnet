using Flashcards.Api.BL.Services.Interfaces;
using Flashcards.Api.DAL.Common.Entities;
using Flashcards.Api.DAL.Common.Repositories;

namespace Flashcards.Api.BL.Services
{
    public class CurrentUserProvider(IUserContext userContext, IUserRepository userRepository) : ICurrentUserProvider
    {
        public UserEntity GetCurrentUser()
        {
            var identityId = userContext.CurrentIdentityId;

            var user = userRepository.GetByIdentityId(identityId);
            return user ?? throw new UnauthorizedAccessException("Current user record not found.");
        }
    }
}
