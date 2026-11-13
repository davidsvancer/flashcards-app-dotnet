using Flashcards.Api.DAL.Common.Entities;
using Flashcards.Api.DAL.Common.Repositories;
using Flashcards.Api.DAL.Memory.Repositories;
using Flashcards.Common.Enums;

namespace Flashcards.Api.DAL.IntegrationTests
{
    public class CardCollectionRepositoryTests
    {
        private readonly IFakeDatabase _fakeDb;
        private readonly ICardCollectionRepository _repo;

        public CardCollectionRepositoryTests()
        {
            _fakeDb = new InMemoryFakeDatabase();
            _repo = _fakeDb.GetCardCollectionRepository();
        }

        private bool AreCollectionsEqual(CardCollectionEntity c1, CardCollectionEntity c2)
        {
            if (c1.Id != c2.Id) return false;
            if (c1.Title != c2.Title) return false;
            if (c1.AnswerStart != c2.AnswerStart) return false;
            if (c1.AnswerEnd != c2.AnswerEnd) return false;

            // compare card lists by ids and count
            if ((c1.Cards == null) != (c2.Cards == null)) return false;
            if (c1.Cards == null && c2.Cards == null) return true;

            if (c1.Cards.Count != c2.Cards.Count) return false;
            var ids1 = c1.Cards.Select(x => x.Id).OrderBy(x => x).ToList();
            var ids2 = c2.Cards.Select(x => x.Id).OrderBy(x => x).ToList();
            return ids1.SequenceEqual(ids2);
        }

        [Fact]
        public void GetById_Existing_Collection()
        {
            // Act
            var requestedId = _fakeDb.CardCollectionGuids[0];
            var collection = _repo.GetById(requestedId);

            // Assert
            Assert.NotNull(collection);
            Assert.Equal(collection.Id, requestedId);

            var expectedCollection = _fakeDb.GetCardCollectionById(requestedId);
            Assert.NotNull(expectedCollection);
            Assert.True(AreCollectionsEqual(collection, expectedCollection));
        }

        [Fact]
        public void GetById_Non_Existing_Collection()
        {
            // Arrange
            var nonExistingId = Guid.NewGuid();

            // Act
            var collection = _repo.GetById(nonExistingId);

            // Assert
            Assert.Null(collection);
        }

        [Fact]
        public void Delete_Existing_Collection()
        {
            // Arrange
            var idToRemove = _fakeDb.CardCollectionGuids[0];

            // Act
            _repo.Remove(idToRemove);

            // Assert
            Assert.Null(_fakeDb.GetCardCollectionById(idToRemove));
        }

        [Fact]
        public void Insert_New_Collection()
        {
            // Arrange
            var newCollection = new CardCollectionEntity
            {
                Id = Guid.NewGuid(),
                Title = "Math Formulas",
                AnswerStart = DateTime.UtcNow,
                AnswerEnd = DateTime.UtcNow.AddHours(2),
                Cards = new List<CardEntity> {  }
            };

            // Act
            var newId = _repo.Insert(newCollection);

            // Assert
            var inserted = _fakeDb.GetCardCollectionById(newId);
            Assert.NotNull(inserted);
            Assert.Equal(newCollection.Title, inserted.Title);
            Assert.Equal(newCollection.AnswerStart, inserted.AnswerStart);
            Assert.Equal(newCollection.AnswerEnd, inserted.AnswerEnd);
        }

        [Fact]
        public void Update_Existing_Collection()
        {
            // Arrange
            var collectionId = _fakeDb.CardCollectionGuids[0];
            var stored = _repo.GetById(collectionId);
            Assert.NotNull(stored);

            // create detached payload to simulate update from outside storage
            var updatePayload = new CardCollectionEntity
            {
                Id = stored.Id,
                Title = "Updated Collection Title",
                AnswerStart = stored.AnswerStart,
                AnswerEnd = stored.AnswerEnd,
                Cards = stored.Cards
            };

            // Act
            _repo.Update(updatePayload);

            // Assert
            var expectedCollection = _fakeDb.GetCardCollectionById(collectionId);
            Assert.NotNull(expectedCollection);
            Assert.Equal(updatePayload.Title, expectedCollection.Title);
        }
    }
}
