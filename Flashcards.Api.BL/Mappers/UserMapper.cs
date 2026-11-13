using Flashcards.Api.DAL.Common.Entities;
using Flashcards.Common.Models;

namespace Flashcards.Api.BL.Mappers
{
    public class UserMapper
    {
        public UserEntity ToEntity(UserCreateModel model, string ownerId)
        {
            return new UserEntity
            {
                Id = Guid.NewGuid(),
                Name = model.Name,
                PhotoUrl = model.PhotoUrl,
                Role = model.Role,
                IdentityOwnerId = ownerId
            };
        }

        public UserDetailModel ToDetailModel(UserEntity model)
        {
            return new UserDetailModel
            {
                Id = model.Id,
                Name = model.Name,
                PhotoUrl = model.PhotoUrl,
                IsDeleted = model.IsDeleted,
                Role = model.Role
            };
        }

        public List<UserListModel> ToListModel(IEnumerable<UserEntity> entities)
        {
            return entities.Select(ToListModelSingle).ToList();
        }

        public UserListModel ToListModelSingle(UserEntity entity)
        {
            return new UserListModel()
            {
                Id = entity.Id,
                Name = entity.Name,
                IsDeleted = entity.IsDeleted,
                Role = entity.Role
            };
        }
    }
}