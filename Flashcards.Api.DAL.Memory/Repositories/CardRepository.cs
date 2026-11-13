using Flashcards.Api.DAL.Common.Entities;
using Flashcards.Api.DAL.Common.Repositories;
using System.Linq.Expressions;
using Flashcards.Api.DAL.Common.Interfaces;

namespace Flashcards.Api.DAL.Memory.Repositories
{
    public class CardRepository(IStorage storage) : ICardRepository
    {
        public bool Exists(Guid id)
        {
            return storage.Cards.Any(card => card.Id == id);
        }

        public CardEntity? FirstOrDefault(Expression<Func<CardEntity, bool>> predicate)
        {
            return storage.Cards.AsQueryable().FirstOrDefault(predicate);
        }

        public IList<CardEntity> GetAll()
        {
            return storage.Cards.ToList();
        }

        public CardEntity? GetById(Guid id)
        {
            return storage.Cards.SingleOrDefault(card => card.Id == id);
        }

        public Guid Insert(CardEntity card)
        {
            storage.Cards.Add(card);
            return card.Id;
        }

        public void Remove(Guid id)
        {
            var cardToRemove = storage.Cards.Single(card => card.Id == id);
            storage.Cards.Remove(cardToRemove);
        }

        public Guid? Update(CardEntity card)
        {
            var existingCard = storage.Cards.SingleOrDefault(cardInStorage => cardInStorage.Id == card.Id);
            if (existingCard is null)
            {
                return null;
            }

            existingCard.AnswerContent = card.AnswerContent;
            existingCard.QuestionContent = card.QuestionContent;
            existingCard.Created = card.Created;
            existingCard.AnswerType = card.AnswerType;
            existingCard.QuestionType = card.QuestionType;
            existingCard.Description = card.Description;
            return existingCard.Id;
        }

        public IEnumerable<CardEntity> Where(Expression<Func<CardEntity, bool>> predicate)
        {
            return storage.Cards.AsQueryable().Where(predicate);
        }
    }
}
