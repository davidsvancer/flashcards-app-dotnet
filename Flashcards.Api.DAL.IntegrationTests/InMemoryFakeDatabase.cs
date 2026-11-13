using Flashcards.Api.DAL.Common.Entities;
using Flashcards.Api.DAL.Common.Repositories;
using Flashcards.Api.DAL.Memory;
using Flashcards.Api.DAL.Memory.Repositories;
using Flashcards.Common.Enums;

namespace Flashcards.Api.DAL.IntegrationTests
{
    public class InMemoryFakeDatabase : IFakeDatabase
    {
        private readonly Storage inMemoryStorage;

        public InMemoryFakeDatabase()
        {
            inMemoryStorage = new Storage(createWithDummyData: false);
            SeedStorage();
        }

        public ICardRepository GetCardRepository()
        {
            return new CardRepository(inMemoryStorage);
        }
        
        public CardEntity GetCardById(Guid id)
        {
            return inMemoryStorage.Cards.SingleOrDefault(card => card.Id == id);
        }
        
        public IList<Guid> CardGuids { get; } = new List<Guid>
        {
            new("3b07c916-a86e-4992-a58b-0158d3232335"),
            new("3807e412-bf2b-4825-b361-039215cb4eaa"),
            new("803a24c3-2c93-4fc8-b96d-4c8f5ee63ecf")
        };
        
        public IUserRepository GetUserRepository()
        {
            return new UserRepository(inMemoryStorage);
        }

        public UserEntity? GetUserById(Guid id)
        {
            return inMemoryStorage.Users.SingleOrDefault(user => user.Id == id);
        }

        public IList<UserEntity> GetAll()
        {
            return inMemoryStorage.Users.ToList();
        }
        
        public IList<Guid> UserGuids { get; } = new List<Guid>
        {
            new("f98b43b3-2c65-4e84-9538-094e7dd08a3d"),
            new("6a2cabfb-b818-4fac-a6b3-1b865536d8a6"),
            new("bd4422e9-f4b3-43d7-a8ce-165aa8a021ff"),
            new("5b6574e9-037b-422e-8c5c-f421e4f2c82b")
        };

        public ICardCollectionRepository GetCardCollectionRepository()
        {
            return new CardCollectionRepository(inMemoryStorage);
        }

        public CardCollectionEntity GetCardCollectionById(Guid id)
        {
            return inMemoryStorage.Collections.SingleOrDefault(collection => collection.Id == id);
        }

        public IList<Guid> CardCollectionGuids { get; } = new List<Guid>
        {
            new("a6b56d25-89e0-41f5-b785-5c1a44a5c54d"),
            new("e1f4b0f7-9db1-4877-812f-1cc1e4fbc7d8"),
            new("c74d2e0c-5989-4f6b-a83c-fb0a3c514f6d")

        };

        public ILessonRepository GetLessonRepository()
        {
            return new LessonRepository(inMemoryStorage);
        }

        public LessonEntity GetLessonById(Guid id)
        {
            return inMemoryStorage.Lessons.SingleOrDefault(lesson => lesson.Id == id);
        }

        public IList<Guid> LessonGuids { get; } = new List<Guid>
        {
            new("a1f3c9d2-4b7e-4e9a-8c5f-2d1b6a9f8e11"),
            new("b2d4e1f3-7c8a-45b2-9f6d-3c2a7e0b9d22"),
            new("c3e5f2a4-1d9b-46c3-8a7e-4d3b8f1c0e33")

        };

        private void SeedStorage()
        {
            inMemoryStorage.Cards.Add(new CardEntity
            {
                Id = CardGuids[0],
                QuestionType = ContentType.Text,
                AnswerType = ContentType.Text,
                QuestionContent = "Who is showing up to friday morning ISA lecture?",
                AnswerContent = "Not me",
                Created = new DateTime(2025, 2, 1, 10,0,0,0),
                Description = "A common sense question."
            });

            inMemoryStorage.Cards.Add(new CardEntity
            {
                Id = CardGuids[1],
                QuestionType = ContentType.Text,
                AnswerType = ContentType.Text,
                QuestionContent = "Best asian food?",
                AnswerContent = "Kottu",
                Created = new DateTime(2025, 2, 2, 11, 0, 0, 0),
                Description = "A general question"
            });

            inMemoryStorage.Cards.Add(new CardEntity
            {
                Id = CardGuids[2],
                QuestionType = ContentType.Text,
                AnswerType = ContentType.Text,
                QuestionContent = "What is the most overhyped programming language?",
                AnswerContent = "Rust",
                Created = new DateTime(2025, 2, 3, 9, 0, 0, 0),
                Description = "A programming question"
            });
            
            inMemoryStorage.Users.Add(new UserEntity
            {
                Id = UserGuids[0],
                Name = "Pavel",
                PhotoUrl = null,
                Role = RoleType.User,
                IdentityOwnerId = "PavelId"
            });
            
            inMemoryStorage.Users.Add(new UserEntity
            {
                Id = UserGuids[1],
                Name = "Oliver",
                PhotoUrl = null,
                Role = RoleType.User,
                IdentityOwnerId = "OliverId"
            });
            
            inMemoryStorage.Users.Add(new UserEntity
            {
                Id = UserGuids[2],
                Name = "Vit",
                PhotoUrl = null,
                Role = RoleType.User,
                IdentityOwnerId = "VitId"
            });
            
            inMemoryStorage.Users.Add(new UserEntity
            {
                Id = UserGuids[3],
                Name = "Adam",
                PhotoUrl = null,
                Role = RoleType.Admin,
                IdentityOwnerId = "AdamId"
            });

            inMemoryStorage.Collections.Add(new CardCollectionEntity
            {
                Id = CardCollectionGuids[0],
                Title = "General Knowledge",
                AnswerStart = new DateTime(2025, 2, 10, 8, 0, 0, 0),
                AnswerEnd = new DateTime(2025, 2, 10, 10, 0, 0, 0),
                Cards = new List<CardEntity> { inMemoryStorage.Cards[0], inMemoryStorage.Cards[1] }
            });

            inMemoryStorage.Collections.Add(new CardCollectionEntity
            {
                Id = CardCollectionGuids[1],
                Title = "Programming Trivia",
                AnswerStart = new DateTime(2025, 2, 11, 9, 0, 0, 0),
                AnswerEnd = new DateTime(2025, 2, 11, 11, 0, 0, 0),
                Cards = new List<CardEntity> { inMemoryStorage.Cards[2] }
            });

            inMemoryStorage.Collections.Add(new CardCollectionEntity
            {
                Id = CardCollectionGuids[2],
                Title = "Random Fun Facts",
                AnswerStart = new DateTime(2025, 2, 12, 8, 30, 0, 0),
                AnswerEnd = new DateTime(2025, 2, 12, 9, 30, 0, 0),
                Cards = new List<CardEntity> { inMemoryStorage.Cards[1] }
            });

            inMemoryStorage.Lessons.Add(new LessonEntity
            {
                Id = LessonGuids[0],
                User = inMemoryStorage.Users[0],
                CardCollection = inMemoryStorage.Collections[0],
                Start = new DateTime(2025, 2, 15, 10, 0, 0),
                Seen = 3,
                Correct = 2,
                Wrong = 1
            });

            inMemoryStorage.Lessons.Add(new LessonEntity
            {
                Id = LessonGuids[1],
                User = inMemoryStorage.Users[1],
                CardCollection = inMemoryStorage.Collections[1],
                Start = new DateTime(2025, 2, 16, 11, 0, 0),
                Seen = 5,
                Correct = 4,
                Wrong = 1
            });

            inMemoryStorage.Lessons.Add(new LessonEntity
            {
                Id = LessonGuids[2],
                User = inMemoryStorage.Users[2],
                CardCollection = inMemoryStorage.Collections[2],
                Start = new DateTime(2025, 2, 17, 9, 30, 0),
                Seen = 2,
                Correct = 2,
                Wrong = 0
            });
        }
    }
}
