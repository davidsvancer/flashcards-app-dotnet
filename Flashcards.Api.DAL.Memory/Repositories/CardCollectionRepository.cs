using Flashcards.Api.DAL.Common.Entities;
using Flashcards.Api.DAL.Common.Repositories;
using System.Linq.Expressions;
using Flashcards.Api.DAL.Common.Interfaces;

namespace Flashcards.Api.DAL.Memory.Repositories
{
    public class CardCollectionRepository(IStorage storage) : ICardCollectionRepository
    {
        public bool Exists(Guid id)
        {
            return storage.Collections.Any(collection => collection.Id == id);
        }

        public CardCollectionEntity? FirstOrDefault(Expression<Func<CardCollectionEntity, bool>> predicate)
        {
            return storage.Collections.AsQueryable().FirstOrDefault(predicate);
        }

        public IList<CardCollectionEntity> GetAll()
        {
            return storage.Collections.ToList();
        }

        public CardCollectionEntity? GetById(Guid id)
        {
            return storage.Collections.SingleOrDefault(collection => collection.Id == id);
        }

        public Guid Insert(CardCollectionEntity collection)
        {
            storage.Collections.Add(collection);
            return collection.Id;
        }

        public void Remove(Guid id)
        {
            var collectionToRemove = storage.Collections.Single(collection => collection.Id == id);
            storage.Collections.Remove(collectionToRemove);
        }

        public Guid? Update(CardCollectionEntity collection)
        {
            var existingCollection = storage.Collections.SingleOrDefault(collectionInStorage => collectionInStorage.Id == collection.Id);
            if (existingCollection is null)
            {
                return null;
            }

            existingCollection.AnswerStart = collection.AnswerStart;
            existingCollection.AnswerEnd = collection.AnswerEnd;
            existingCollection.Title = collection.Title;
            existingCollection.Cards = collection.Cards;
            return existingCollection.Id;
        }

        public IEnumerable<CardCollectionEntity> Where(Expression<Func<CardCollectionEntity, bool>> predicate)
        {
            return storage.Collections.AsQueryable().Where(predicate);
        }
    }
}