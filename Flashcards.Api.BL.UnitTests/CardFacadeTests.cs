using Flashcards.Api.BL.Facades;
using Flashcards.Api.BL.Mappers;
using Flashcards.Api.BL.Services.Interfaces;
using Flashcards.Api.DAL.Common.Entities;
using Flashcards.Api.DAL.Common.Repositories;
using Flashcards.Common.Models;
using Flashcards.Common.Enums;
using NSubstitute;

namespace Flashcards.Api.BL.UnitTests
{
    public class CardFacadeTests
    {
        private readonly ICardRepository _cardRepo;
        private readonly ICurrentUserProvider _userProvider;
        private readonly ISecurityService _securityService;
        private readonly CardFacade _facade;
        private readonly CardMapper _cardMapper;
        private readonly UserMapper _userMapper;

        public CardFacadeTests()
        {
            _userMapper = new UserMapper();
            _cardMapper = new CardMapper(_userMapper);
            _cardRepo = Substitute.For<ICardRepository>();
            _userProvider = Substitute.For<ICurrentUserProvider>();
            _securityService = Substitute.For<ISecurityService>();

            _userProvider.GetCurrentUser().Returns(_ownerEntity);

            _facade = new CardFacade(_cardRepo, _userProvider, _securityService, _cardMapper);
        }
        private CardCreateModel CreateCardCreateModel()
        {
            return new CardCreateModel
            {
                QuestionType = ContentType.Text,
                AnswerType = ContentType.Text,
                QuestionContent = "What is 2 + 2?",
                AnswerContent = "4",
                Description = "A simple math question."
            };
        }

        private CardDetailModel CreateCardDetailModel()
        {
            return new CardDetailModel
            {
                Id = Guid.NewGuid(),
                QuestionType = ContentType.Text,
                AnswerType = ContentType.Text,
                QuestionContent = "What is 2 + 2?",
                AnswerContent = "4",
                DateCreated = DateTime.UtcNow,
                Description = "A simple math question.",
                Owner = new UserListModel { Id = _ownerEntity.Id, Name = _ownerEntity.Name, Role = _ownerEntity.Role, IsDeleted = false }
            };
        }

        private CardEntity CardEntityWithGuid(Guid id) => new()
        {
            Id = id,
            AnswerContent = "A",
            AnswerType = ContentType.Text,
            QuestionContent = "Q",
            QuestionType = ContentType.Text,
            Created = DateTime.MinValue,
            OwnerId = Guid.Empty
        };

        private readonly UserEntity _ownerEntity = new UserEntity
        {
            Id = Guid.NewGuid(),
            IdentityOwnerId = "abc",
            Name = "testuser",
            Role = RoleType.User,
        };

        [Fact]
        public void Delete_Calls_Method_On_Repository_Correctly()
        {
            // Arrange
            var cardGuid = Guid.NewGuid();
            var entity = CardEntityWithGuid(cardGuid);
            _cardRepo.GetById(cardGuid).Returns(entity);
            
            // Act
            _facade.Delete(cardGuid);

            // Assert
            _cardRepo.Received().Remove(cardGuid);
        }

        [Fact]
        public void Create_New_Card()
        {
            // Arrange
            var newCard = CreateCardCreateModel();

            // Act
            var result = _facade.Create(newCard);

            // Assert
            _cardRepo.Received(1).Insert(Arg.Is<CardEntity>(card => 
                card.Id == result && 
                card.Owner != null && 
                card.Owner.IdentityOwnerId == _ownerEntity.IdentityOwnerId)
            );
        }

        [Fact]
        public void Create_New_Card_With_CreateOrUpdate()
        {
            // Arrange
            var newCard = CreateCardDetailModel();

            // Act
            var result = _facade.CreateOrUpdate(newCard);

            // Assert
            _cardRepo.Received(1).Insert(Arg.Is<CardEntity>(card => card.Id == result));
            Assert.Equal(result, result);
        }

        [Fact]
        public void Update_Existing_Card_With_CreateOrUpdate()
        {
            // Arrange
            var existingCard = CreateCardDetailModel();
            _cardRepo.Exists(existingCard.Id).Returns(true);

            var existingEntity = _cardMapper.ToEntity(existingCard, _ownerEntity);
            _cardRepo.GetById(existingCard.Id).Returns(existingEntity);
            _cardRepo.Update(Arg.Any<CardEntity>()).Returns(existingEntity.Id);
            
            existingCard.Description = "updated value";

            // Act
            var result = _facade.CreateOrUpdate(existingCard);
            
            // Assert
            _cardRepo.DidNotReceive().Insert(Arg.Any<CardEntity>());
            _cardRepo.Received(1).Update(Arg.Is<CardEntity>(
                card => card.Id == existingCard.Id && card.Description == "updated value"));
            
            Assert.Equal(existingCard.Id, result);
        }

        [Fact]
        public void GetById_On_Existing_Card()
        {
            // Arrange
            var newCard = CreateCardDetailModel();
            _cardRepo.GetById(Arg.Any<Guid>()).Returns(_cardMapper.ToEntity(newCard, _ownerEntity));
            
            // Act
            var result = _facade.GetById(newCard.Id);
            
            // Assert
            _cardRepo.Received(1).GetById(newCard.Id);
            Assert.NotNull(result);
            Assert.Equal(newCard.Id, result.Id);
        }
        
        [Fact]
        public void GetById_On_Non_Existing_Card()
        {
            // Arrange
            var nonExistingId = Guid.NewGuid();
            _cardRepo.GetById(Arg.Any<Guid>()).Returns((CardEntity?)null);
            
            // Act
            var result = _facade.GetById(nonExistingId);
            
            // Assert
            _cardRepo.Received(1).GetById(nonExistingId);
            Assert.Null(result);
        }

        [Fact]
        public void GetAll_On_Empty_Repo()
        {
            // Arrange
            _cardRepo.GetAll().Returns(new List<CardEntity>());

            // Act
            var result = _facade.GetAll();

            // Assert
            _cardRepo.Received(1).GetAll();
            Assert.Empty(result);
        }
        
        [Fact]
        public void GetAll_On_Non_Empty_Repo()
        {
            // Arrange
            var newCard = CreateCardDetailModel();
            var entities = new List<CardEntity>{ _cardMapper.ToEntity(newCard, _ownerEntity) };
            _cardRepo.GetAll().Returns(entities);

            // Act
            var result = _facade.GetAll();

            // Assert
            _cardRepo.Received(1).GetAll();
            Assert.NotEmpty(result);
        }
        
        [Fact]
        public void Update_Existing_Card()
        {
            // Arrange
            var updatedCard = CreateCardDetailModel();

            var existingCardEntity = _cardMapper.ToEntity(updatedCard, _ownerEntity);
            _cardRepo.GetById(updatedCard.Id).Returns(existingCardEntity);

            // change something in the updated card
            updatedCard.Description = "updated value";

            _cardRepo.Update(Arg.Any<CardEntity>()).Returns(updatedCard.Id);

            // Act
            var result = _facade.Update(updatedCard);
            
            // Assert
            _cardRepo.DidNotReceive().Insert(Arg.Any<CardEntity>());
            _cardRepo.Received(1).Update(Arg.Is<CardEntity>(
                card => card.Id == updatedCard.Id && card.Description == "updated value"));
            
            Assert.Equal(updatedCard.Id, result);
        }
        
        [Fact]
        public void Update_On_Non_Existing_Card()
        {
            // Arrange
            var card = CreateCardDetailModel();

            CardEntity? nonExistingCardEntity = null;
            _cardRepo.GetById(card.Id).Returns(nonExistingCardEntity);

            // Act
            var result = _facade.Update(card);
            
            // Assert
            _cardRepo.DidNotReceive().Insert(Arg.Any<CardEntity>());
            _cardRepo.DidNotReceive().Update(Arg.Any<CardEntity>());
            Assert.Null(result);
        }

        [Fact]
        public void GetByQuery_Filters_By_Question_Text()
        {
            // Arrange
            var cards = new List<CardEntity>()
            {
                new()
                {
                    Id = new("7f251cd6-3ac4-49be-b3e7-d1f9f7cfdd3a"),
                    QuestionType = ContentType.Text,
                    AnswerType = ContentType.Text,
                    QuestionContent = "What is the capital of France?",
                    AnswerContent = "Paris",
                    Created = DateTime.UtcNow,
                    Description = "A geography question."
                },
                new()
                {
                    Id = new("23b3902d-7d4f-4213-9cf0-112348f56238"),
                    QuestionType = ContentType.Text,
                    AnswerType = ContentType.Text,
                    QuestionContent = "What is 2 + 2?",
                    AnswerContent = "4",
                    Created = DateTime.UtcNow,
                    Description = "A simple math question."
                },
                new()
                {
                    Id = new("df935095-8709-4040-a2bb-b6f97cb416dc"),
                    QuestionType = ContentType.Text,
                    AnswerType = ContentType.Text,
                    QuestionContent = "Who wrote 'Hamlet'?",
                    AnswerContent = "William Shakespeare",
                    Created = DateTime.UtcNow,
                    Description = "A literature question."
                }
            };
            _cardRepo.GetAll().Returns(cards);

            var query = new CardQueryModel() { SearchString = "What" };
            var filtered = _facade.GetByQuery(query);

            Assert.NotNull(filtered);
            Assert.Equal(2, filtered.Items.Count);
        }

        [Fact]
        public void GetByQuery_Filters_By_QuestionTextAndDescription()
        {
            var cards = new List<CardEntity>()
            {
                new()
                {
                    Id = Guid.Empty,
                    QuestionType = ContentType.Text,
                    AnswerType = ContentType.Text,
                    QuestionContent = "Question1",
                    AnswerContent = "Answer1",
                    Created = DateTime.UtcNow,
                    Description = "Desc1."
                },
                new()
                {
                    Id = Guid.Empty,
                    QuestionType = ContentType.Text,
                    AnswerType = ContentType.Text,
                    QuestionContent = "Question2FINDME",
                    AnswerContent = "Answer2",
                    Created = DateTime.UtcNow,
                    Description = "Desc3"
                },
                new()
                {
                    Id = Guid.Empty,
                    QuestionType = ContentType.Text,
                    AnswerType = ContentType.Text,
                    QuestionContent = "Question3",
                    AnswerContent = "Answer3",
                    Created = DateTime.UtcNow,
                    Description = "Desc3FINDME_"
                }
            };

            _cardRepo.GetAll().Returns(cards);

            var query = new CardQueryModel() { SearchString = "FINDME" };
            var filtered = _facade.GetByQuery(query);

            Assert.NotNull(filtered);
            Assert.Equal(2, filtered.Items.Count());
            // Assert Questions 2 and 3 are in the result:
            Assert.Contains(filtered.Items, card => card.QuestionContent == "Question2FINDME");
            Assert.Contains(filtered.Items, card => card.QuestionContent == "Question3");
        }
    }
}
