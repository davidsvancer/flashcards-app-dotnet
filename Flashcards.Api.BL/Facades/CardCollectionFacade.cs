using Flashcards.Api.BL.Mappers;
using Flashcards.Api.BL.Services.Interfaces;
using Flashcards.Api.DAL.Common.Entities;
using Flashcards.Api.DAL.Common.Repositories;
using Flashcards.Common.Models;

namespace Flashcards.Api.BL.Facades
{
    public class CardCollectionFacade(
        ICardCollectionRepository cardCollectionRepository,
        ISecurityService securityService,
        ICurrentUserProvider currentUser,
        CardCollectionMapper mapper)
        : ICardCollectionFacade
    {
        public List<CardCollectionListModel> GetAll()
        {
            var entities = cardCollectionRepository.GetAll();
            return mapper.ToListModel(entities);
        }

        public PagedResponse<CardCollectionListModel> GetByQuery(CardCollectionQueryModel query)
        {
            var collection = cardCollectionRepository.GetAll().AsEnumerable();

            if (!string.IsNullOrEmpty(query.SearchString))
            {
                collection = collection.Where(c =>
                    c.Title.Contains(query.SearchString, StringComparison.OrdinalIgnoreCase));
            }

            if (query.OwnerId is not null)
            {
                collection = collection.Where(c => c.OwnerId == query.OwnerId);
            }

            if (query.ExcludeOwnerId is not null)
            {
                collection = collection.Where(c => c.OwnerId != query.ExcludeOwnerId);
            }

            if (query.ShowCurrentlyAvailableOnly)
            {
                var now = DateTime.Now;
                collection = collection.Where(c => c.AnswerStart <= now && now <= c.AnswerEnd);
            }

            var totalCount = collection.Count();
            collection = QueryHelper.PerformPagination(collection, query);
            var result = mapper.ToListModel(collection.ToList());

            return new PagedResponse<CardCollectionListModel>(result, totalCount, query.Page, query.PageSize);
        }

        public CardCollectionDetailModel? GetById(Guid id)
        {
            var entity = cardCollectionRepository.GetById(id);
            return entity == null ? null : mapper.ToDetailModel(entity);
        }

        public Guid CreateOrUpdate(CardCollectionDetailModel model)
        {
            return cardCollectionRepository.Exists(model.Id)
                ? Update(model)!.Value
                : Create(mapper.ToCreateModel(model));
        }

        public Guid Create(CardCollectionCreateModel model)
        {
            var requestorUserEntity = currentUser.GetCurrentUser();

            var entity = mapper.ToEntity(model, requestorUserEntity);
            return cardCollectionRepository.Insert(entity);
        }

        public Guid? Update(CardCollectionDetailModel model)
        {
            var existing = cardCollectionRepository.GetById(model.Id);
            if (existing is null)
                return null;
            
            securityService.ThrowIfNotOwnerAndAdmin(existing);

            var ownerEntity = currentUser.GetCurrentUser();
            var entity = mapper.ToEntity(model, ownerEntity!);

            return cardCollectionRepository.Update(entity);
        }

        public void Delete(Guid id)
        {
            var collectionToDelete = cardCollectionRepository.GetById(id);
            if (collectionToDelete is null)
                return;

            securityService.ThrowIfNotOwnerAndAdmin(collectionToDelete);

            cardCollectionRepository.Remove(id);
        }
    }
}
