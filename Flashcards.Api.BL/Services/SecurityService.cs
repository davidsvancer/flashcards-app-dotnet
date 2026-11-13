using Flashcards.Api.BL.Services.Interfaces;
using Flashcards.Api.DAL.Common.Entities;
using Flashcards.Api.DAL.Common.Entities.Interfaces;
using Flashcards.Api.DAL.Common.Repositories;

namespace Flashcards.Api.BL.Services
{
    public class SecurityService(IUserRepository userRepository, IUserContext userContext) : ISecurityService
    {
        public void ThrowIfNotAdmin()
        {
            if (!userContext.IsAdmin)
            {
                throw new UnauthorizedAccessException($"There is no App User account assigned to this Identity ID:{userContext.CurrentIdentityId}");
            }
        }

        public void ThrowIfNotOwnerAndAdmin(IOwnedEntity resource)
        {
            if (userContext.IsAdmin)
            {
                // Admin is authorized to do anything
                return;
            }

            var requestorUserEntity = GetRequestorsUserEntity();

            // Is regular user, must be an owner of that Entity
            if (requestorUserEntity.Id != resource.OwnerId)
            {
                throw new UnauthorizedAccessException(
                    "Only an owner of this entity or admin can perform this operation");
            }
        }

        public void TrowIfUnknownUser()
        {
            var requestorUserEntity = userRepository.GetByIdentityId(userContext.CurrentIdentityId);
            if (requestorUserEntity is null)
            {
                throw new UnauthorizedAccessException(
                    $"There is no App User account assigned to this Identity ID:{userContext.CurrentIdentityId}");
            }
        }

        public void ThrowIfUnauthorizedUserUpdate(Guid targetUserId)
        {
            if (userContext.IsAdmin)
            {
                // Admin is authorized to do anything
                return;
            }

            var requestorUserEntity = GetRequestorsUserEntity();

            if (targetUserId != requestorUserEntity.Id)
            {
                throw new UnauthorizedAccessException("Cannot edit another users profile.");
            }
        }

        private UserEntity GetRequestorsUserEntity()
        {
            var requestorId = userContext.CurrentIdentityId;
            var requestorUserEntity = userRepository.GetByIdentityId(userContext.CurrentIdentityId);
            if (requestorUserEntity is null)
            {
                throw new UnauthorizedAccessException(
                    $"There is no App User account assigned to this Identity ID:{requestorId}");
            }

            return requestorUserEntity;
        }
    }
}
