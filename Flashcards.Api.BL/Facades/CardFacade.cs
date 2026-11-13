using Flashcards.Api.BL.Mappers;
using Flashcards.Api.BL.Services.Interfaces;
using Flashcards.Api.DAL.Common.Entities;
using Flashcards.Api.DAL.Common.Repositories;
using Flashcards.Common.Models;

namespace Flashcards.Api.BL.Facades
{
    public class CardFacade(ICardRepository cardRepository, ICurrentUserProvider currentUser, ISecurityService securityService, CardMapper mapper)
       : ICardFacade
    {
        public Guid Create(CardCreateModel card)
        {
            var requestorUserEntity = currentUser.GetCurrentUser();

            var cardEntity = mapper.ToEntity(card, requestorUserEntity);
            cardRepository.Insert(cardEntity);
            return cardEntity.Id;
        }

        public Guid CreateOrUpdate(CardDetailModel card)
        {
            return cardRepository.Exists(card.Id)
                ? Update(card)!.Value
                : Create(mapper.ToCreateModel(card));
        }

        public void Delete(Guid id)
        {
            var cardToDelete = cardRepository.GetById(id);
            if (cardToDelete is null)
            {
                return;
            }

            securityService.ThrowIfNotOwnerAndAdmin(cardToDelete);

            cardRepository.Remove(id);
        }

        public List<CardListModel> GetAll()
        {
            var entities = cardRepository.GetAll();
            return mapper.ToListModel(entities);
        }

        public PagedResponse<CardListModel> GetByQuery(CardQueryModel query)
        {
            var collection = cardRepository.GetAll().AsEnumerable();

            if (!string.IsNullOrEmpty(query.SearchString))
            {
                var search = query.SearchString.Trim();
                collection = collection.Where(c =>
                    (c.QuestionContent.Contains(search, StringComparison.OrdinalIgnoreCase)) ||
                    (c.Description != null && c.Description.Contains(search, StringComparison.OrdinalIgnoreCase))
                );
            }

            if (query.QuestionType is not null)
            {
                collection = collection.Where(c => c.QuestionType == query.QuestionType);
            }

            if (query.AnswerType is not null)
            {
                collection = collection.Where(c => c.AnswerType == query.AnswerType);
            }

            if (query.OwnerId is not null)
            {
                collection = collection.Where(c => c.Owner != null && c.Owner.Id == query.OwnerId);
            }

            var totalCount = collection.Count();
            collection = QueryHelper.PerformPagination(collection, query);
            var result = mapper.ToListModel(collection.ToList());

            return new PagedResponse<CardListModel>(result, totalCount, query.Page, query.PageSize);
        }

        public CardDetailModel? GetById(Guid id)
        {
            var cardEntity = cardRepository.GetById(id);
            return cardEntity is null 
                ? null 
                : mapper.ToDetailModel(cardEntity);
        }

        public Guid? Update(CardDetailModel card)
        {
            var cardToUpdate = cardRepository.GetById(card.Id);
            if (cardToUpdate is null)
            {
                return null;
            }

            securityService.ThrowIfNotOwnerAndAdmin(cardToUpdate);

            var cardEntity = new CardEntity()
            {
                Id = card.Id,
                QuestionType = card.QuestionType,
                AnswerType = card.AnswerType,
                QuestionContent = card.QuestionContent,
                AnswerContent = card.AnswerContent,
                Created = card.DateCreated,
                Description = card.Description,
                Owner = cardToUpdate.Owner,
                OwnerId = cardToUpdate.OwnerId
            };
            return cardRepository.Update(cardEntity);
        }
    }
}
