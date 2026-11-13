using Flashcards.Api.DAL.Common.Entities.Interfaces;

namespace Flashcards.Api.BL.Services.Interfaces
{
    public interface ISecurityService
    {
        void ThrowIfNotAdmin();
        void ThrowIfNotOwnerAndAdmin(IOwnedEntity resource);

        void ThrowIfUnauthorizedUserUpdate(Guid targetUserId);
        void TrowIfUnknownUser();
    }
}
