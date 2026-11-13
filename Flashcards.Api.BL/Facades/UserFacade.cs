using Flashcards.Api.BL.Mappers;
using Flashcards.Api.BL.Services.Interfaces;
using Flashcards.Api.DAL.Common.Entities;
using Flashcards.Api.DAL.Common.Repositories;
using Flashcards.Common.Enums;
using Flashcards.Common.Models;

namespace Flashcards.Api.BL.Facades
{
    public class UserFacade(
        IUserRepository userRepository,
        ILessonRepository lessonRepository,
        ISecurityService securityService,
        IIdentityService identityService,
        UserMapper mapper) : IUserFacade
    {
        public List<UserListModel> GetAll()
        {
            var entities = userRepository.GetAll();
            return mapper.ToListModel(entities);
        }

        public PagedResponse<UserListModel> GetByQuery(UserQueryModel query)
        {
            var collection = userRepository.GetAll().AsEnumerable();

            if (query.Role is not null)
            {
                collection = collection.Where(u => u.Role == query.Role);
            }

            if (!string.IsNullOrEmpty(query.SearchString))
            {
                collection = collection.Where(u =>  
                    u.Name.Contains(query.SearchString, StringComparison.OrdinalIgnoreCase));
            }

            var totalCount = collection.Count();
            collection = QueryHelper.PerformPagination(collection, query);
            var result = mapper.ToListModel(collection.ToList());

            return new PagedResponse<UserListModel>(result, totalCount, query.Page, query.PageSize);
        }

        public UserDetailModel? GetById(Guid id)
        {
            var entity = userRepository.GetById(id);
            return entity == null ? null : mapper.ToDetailModel(entity);
        }

        public UserDetailModel? GetByIdentityId(string identityId)
        {
            var entity = userRepository.GetByIdentityId(identityId);
            return entity == null ? null : mapper.ToDetailModel(entity);
        }

        public Guid Create(UserCreateModel model)
        {
            securityService.ThrowIfNotAdmin();

            var role = model.Role == RoleType.Admin ? "Admin" : "User";
            var subId = identityService.CreateIdentityAsync(model.IdentityName, model.Password, role).Result;
            if (string.IsNullOrWhiteSpace(subId))
                throw new InvalidOperationException("Failed to create identity for the user.");

            var entity = mapper.ToEntity(model, subId);
            return userRepository.Insert(entity);
        }

        public Guid? Update(UserUpdateModel model)
        {
            securityService.ThrowIfUnauthorizedUserUpdate(model.Id);

            var existing = userRepository.GetById(model.Id);
            if (existing == null)
                return null;

            var updated = new UserEntity()
            {
                Id = model.Id,
                IdentityOwnerId = existing.IdentityOwnerId, // keep
                IsDeleted = existing.IsDeleted, // keep
                Name = model.Name,
                Role = existing.Role, // keep
                PhotoUrl = model.PhotoUrl,
            };

            return userRepository.Update(updated);
        }

        public void Delete(Guid id)
        {
            securityService.ThrowIfNotAdmin();

            var deletedUserEntity = userRepository.GetById(id);
            if (deletedUserEntity is null || deletedUserEntity.IsDeleted)
                return;
            
            // Delete users Identity
            identityService.DeleteIdentityAsync(deletedUserEntity.IdentityOwnerId);

            // Mark user as deleted by removing IdentityOwnerId
            var userWithDeletedIdentity = new UserEntity()
            {
                Id = deletedUserEntity.Id,
                Name = deletedUserEntity.Name,
                Role = deletedUserEntity.Role,
                IsDeleted = true,
                IdentityOwnerId = string.Empty
            };
            userRepository.Update(userWithDeletedIdentity);

            // Delete users lessons:
            var lessonsOfDeletedUser = lessonRepository.Where(lesson => lesson.OwnerId == deletedUserEntity.Id);
            foreach(var lesson in lessonsOfDeletedUser)
            {
                lessonRepository.Remove(lesson.Id);
            }
        }
    }
}