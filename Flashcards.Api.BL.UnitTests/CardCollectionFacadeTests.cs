using Flashcards.Api.BL.Facades;
using Flashcards.Api.BL.Mappers;
using Flashcards.Api.BL.Services.Interfaces;
using Flashcards.Api.DAL.Common.Entities;
using Flashcards.Api.DAL.Common.Repositories;
using Flashcards.Common.Enums;
using Flashcards.Common.Models;
using NSubstitute;

namespace Flashcards.Api.BL.UnitTests
{
    public class CardCollectionFacadeTests
    {
        private readonly ICardCollectionRepository _fakeRepo;
        private readonly ICurrentUserProvider _userProvider;
        private readonly ISecurityService _securityService;
        private readonly CardCollectionMapper _mapper;
        private readonly CardCollectionFacade _facade;
        private readonly UserEntity _ownerEntity;

        public CardCollectionFacadeTests()
        {
            _fakeRepo = Substitute.For<ICardCollectionRepository>();
            _userProvider = Substitute.For<ICurrentUserProvider>();
            _securityService = Substitute.For<ISecurityService>();

            var userMapper = new UserMapper();
            var cardMapper = new CardMapper(userMapper);
            _mapper = new CardCollectionMapper(cardMapper, userMapper);

            _ownerEntity = new UserEntity
            {
                Id = Guid.NewGuid(),
                IdentityOwnerId = "abc",
                Name = "testuser",
                Role = RoleType.User
            };

            _userProvider.GetCurrentUser().Returns(_ownerEntity);

            _facade = new CardCollectionFacade(_fakeRepo, _securityService, _userProvider, _mapper);
        }

        private CardCollectionDetailModel CreateCardCollectionDetailModel(Guid id)
        {
            var newCardCollection = new CardCollectionDetailModel
            {
                Id = id,
                Title = "Science Deck",
                AnswerStart = DateTime.UtcNow,
                AnswerEnd = DateTime.UtcNow.AddMinutes(15),
                Cards = new List<CardDetailModel>()
            };

            return newCardCollection;
        }
        
        
        [Fact]
        public void Delete_Calls_Correct_Method_On_Repository()
        {
            // Arrange
            var collectionId = Guid.NewGuid();

            var entity = _mapper.ToEntity(CreateCardCollectionDetailModel(collectionId), _ownerEntity);
            _fakeRepo.GetById(collectionId).Returns(entity);

            // Act
            _facade.Delete(collectionId);

            // Assert
            _fakeRepo.Received(1).Remove(collectionId);
        }

        [Fact]
        public void Create_New_Collection()
        {
            // Arrange
            var newCollection = CreateCardCollectionDetailModel(Guid.NewGuid());
            var createModel = new CardCollectionCreateModel
            {
                Title = newCollection.Title,
                AnswerStart = newCollection.AnswerStart,
                AnswerEnd = newCollection.AnswerEnd,
                Cards = new List<CardCreateModel>()
            };

            // Act
            _facade.Create(createModel);

            // Assert
            _fakeRepo.Received(1).Insert(Arg.Is<CardCollectionEntity>(c =>
                c.Title == newCollection.Title &&
                c.AnswerStart == newCollection.AnswerStart &&
                c.AnswerEnd == newCollection.AnswerEnd));
        }

        [Fact]
        public void CreateOrUpdate_On_New_Collection()
        {
            // Arrange
            var newCollection = CreateCardCollectionDetailModel(Guid.NewGuid());

            // Act
            var id = _facade.CreateOrUpdate(newCollection);

            // Assert
            _fakeRepo.Received(1).Insert(Arg.Is<CardCollectionEntity>(c =>
                c.Title == newCollection.Title));
        }

        [Fact]
        public void CreateOrUpdate_On_Existing_Collection()
        {
            var existing = CreateCardCollectionDetailModel(Guid.NewGuid());
            // Arrange
            _fakeRepo.Exists(Arg.Any<Guid>()).Returns(true);
            _fakeRepo.GetById(existing.Id).Returns(_mapper.ToEntity(existing, _ownerEntity)); //added
            _fakeRepo.Update(Arg.Any<CardCollectionEntity>()).Returns(Guid.NewGuid());

            
            existing.Title = "Updated Science Deck";

            // Act
           _facade.CreateOrUpdate(existing);

            // Assert
            _fakeRepo.Received(1).Update(Arg.Is<CardCollectionEntity>(c =>
                c.Id == existing.Id &&
                c.Title == "Updated Science Deck"));
        }

        [Fact]
        public void GetById_NonExisting_Returns_Null()
        {
            // Arrange
            _fakeRepo.GetById(Arg.Any<Guid>()).Returns((CardCollectionEntity?)null);
            var id = Guid.NewGuid();

            // Act
            var result = _facade.GetById(id);

            // Assert
            _fakeRepo.Received(1).GetById(id);
            Assert.Null(result);
        }

        [Fact]
        public void GetById_Existing_Returns_Model()
        {
            // Arrange
            var existing = CreateCardCollectionDetailModel(Guid.NewGuid());
            _fakeRepo.GetById(existing.Id).Returns(_mapper.ToEntity(existing, _ownerEntity));
            
            // Act
            var result = _facade.GetById(existing.Id);

            // Assert
            _fakeRepo.Received(1).GetById(existing.Id);
            Assert.NotNull(result);
            Assert.Equal(existing.Id, result!.Id);
            Assert.Equal(existing.Title, result.Title);
        }

        [Fact]
        public void GetAll_On_Existing_Collections()
        {
            // Arrange

            var col1 = CreateCardCollectionDetailModel(Guid.NewGuid());
            var col2 = CreateCardCollectionDetailModel(Guid.NewGuid());
            
            var list = new List<CardCollectionEntity> { _mapper.ToEntity(col1, _ownerEntity), _mapper.ToEntity(col2, _ownerEntity) };
            _fakeRepo.GetAll().Returns(list);

            // Act
            var result = _facade.GetAll();

            // Assert
            _fakeRepo.Received(1).GetAll();
            Assert.Equal(2, result.Count());
            Assert.Contains(result, c => c.Id == col1.Id && c.Title == col1.Title);
            Assert.Contains(result, c => c.Id == col2.Id && c.Title == col2.Title);
        }

        [Fact]
        public void GetAll_On_Empty_Repo()
        {
            // Arrange
            _fakeRepo.GetAll().Returns(new List<CardCollectionEntity>());
            
            // Act
            var result = _facade.GetAll();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            _fakeRepo.Received(1).GetAll();
        }
    }
}
