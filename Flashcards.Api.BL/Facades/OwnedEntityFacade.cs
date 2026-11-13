
using Flashcards.Api.BL.Services.Interfaces;
using Flashcards.Api.DAL.Common.Entities;
using Flashcards.Api.DAL.Common.Entities.Interfaces;
using Flashcards.Api.DAL.Common.Repositories;
using Flashcards.Common.Models;

namespace Flashcards.Api.BL.Facades
{
    public abstract class BaseOwnedFacade<TEntity, TCreateModel>(
        IApiRepository<TEntity> repository,
        ICurrentUserProvider currentUser,
        ISecurityService security)
        where TEntity : IEntity, IOwnedEntity
    {
        public virtual Guid Create(TCreateModel model)
        {
            var owner = currentUser.GetCurrentUser();

            var entity = MapCreationToEntity(model, owner);

            return repository.Insert(entity);
        }

        public virtual void Delete(Guid id)
        {
            var entity = repository.GetById(id);
            if (entity == null) return;

            security.ThrowIfNotOwnerAndAdmin(entity);

            repository.Remove(entity.Id);
        }

        protected abstract TEntity MapCreationToEntity(TCreateModel model, UserEntity owner);
    }
}
