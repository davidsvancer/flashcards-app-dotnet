using Flashcards.Api.DAL.Common.Entities;
using Flashcards.Api.DAL.Common.Repositories;
using System.Linq.Expressions;
using Flashcards.Api.DAL.Common.Interfaces;

namespace Flashcards.Api.DAL.Memory.Repositories
{
    public class UserRepository(IStorage storage) : IUserRepository
    {
        public bool Exists(Guid id)
        {
            return storage.Users.Any(user => user.Id == id);
        }

        public UserEntity? GetByIdentityId(string identityId)
        {
            var entity = FirstOrDefault(user => string.Equals(user.IdentityOwnerId, identityId));
            if (entity is null)
            {
                throw new ArgumentException("This users identity does not exist!");
            }

            return entity;
        }

        public UserEntity? FirstOrDefault(Expression<Func<UserEntity, bool>> predicate)
        {
            return storage.Users.AsQueryable().FirstOrDefault(predicate);
        }

        public IList<UserEntity> GetAll()
        {
            return storage.Users.ToList();
        }

        public UserEntity? GetById(Guid id)
        {
            return storage.Users.SingleOrDefault(user => user.Id == id);
        }

        public Guid Insert(UserEntity user)
        {
            storage.Users.Add(user);
            return user.Id;
        }

        public void Remove(Guid id)
        {
            var userToRemove = storage.Users.SingleOrDefault(user => user.Id == id);
            if (userToRemove is null)
            {
                return;
            }
            storage.Users.Remove(userToRemove);
        }

        public Guid? Update(UserEntity user)
        {
            var existingUser = storage.Users.SingleOrDefault(userInStorage => userInStorage.Id == user.Id);
            if (existingUser is null)
            {
                return null; // User is not in storage -> can not be updated
            }

            existingUser.Name = user.Name;
            existingUser.Role = user.Role;
            existingUser.PhotoUrl = user.PhotoUrl;
            existingUser.IsDeleted = user.IsDeleted;
            existingUser.IdentityOwnerId = user.IdentityOwnerId;
            return existingUser.Id;
        }

        public IEnumerable<UserEntity> Where(Expression<Func<UserEntity, bool>> predicate)
        {
            return storage.Users.AsQueryable().Where(predicate);
        }

    }
}