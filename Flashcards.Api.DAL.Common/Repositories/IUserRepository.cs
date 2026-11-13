using Flashcards.Api.DAL.Common.Entities;

namespace Flashcards.Api.DAL.Common.Repositories
{
    public interface IUserRepository : IApiRepository<UserEntity>
    {
        public UserEntity? GetByIdentityId(string identityId);
    }
}

