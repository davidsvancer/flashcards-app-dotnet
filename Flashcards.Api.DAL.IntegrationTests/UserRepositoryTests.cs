using Flashcards.Api.DAL.Common.Entities;
using Flashcards.Api.DAL.Common.Repositories;
using Flashcards.Common.Enums;

namespace Flashcards.Api.DAL.IntegrationTests
{
    public class UserRepositoryTests
    {
        private readonly IFakeDatabase _fakeDb;
        private readonly IUserRepository _repo;
        
        public  UserRepositoryTests()
        {
            _fakeDb = new InMemoryFakeDatabase();
            _repo = _fakeDb.GetUserRepository();
        }

        private bool AreUsersEqual(UserEntity user1, UserEntity user2)
        {
            return user1.Id == user2.Id &&
                   user1.Name == user2.Name &&
                   user1.PhotoUrl == user2.PhotoUrl &&
                   user1.IdentityOwnerId == user2.IdentityOwnerId &&
                   user1.Role == user2.Role; 
        }
        
        [Fact]
        public void GetById_Existing_User()
        {
            // Act
            var requestedId = _fakeDb.UserGuids[0];
            var user = _repo.GetById(requestedId);

            // Assert
            Assert.NotNull(user);
            Assert.Equal(user.Id, requestedId);
            
            var expectedUser = _fakeDb.GetUserById(requestedId);
            Assert.NotNull(expectedUser);
            Assert.True(AreUsersEqual(user, expectedUser));
        }

        [Fact]
        public void GetById_Non_Existing_User()
        {
            // Arrange
            var nonExistingId = Guid.NewGuid();
            
            // Act
            var user = _repo.GetById(nonExistingId);
            
            // Assert
            Assert.Null(user);
        }

        [Fact]
        public void Delete_Existing_User()
        {
            // Arrange
            var idToRemove = _fakeDb.UserGuids[0];
            
            // Act
            _repo.Remove(idToRemove);
            
            // Assert
            Assert.Null(_fakeDb.GetUserById(idToRemove));
        }

        [Fact]
        public void Insert_New_User()
        {
            // Arrange
            var newUser = new UserEntity
            {
                Id = Guid.NewGuid(),
                Name = "Kuba",
                PhotoUrl = null,
                Role = RoleType.User,
                IdentityOwnerId = "KubaId"
            };

            // Act
            var newId = _repo.Insert(newUser);

            // Assert
            Assert.NotNull(_fakeDb.GetUserById(newId));
        }

        [Fact]
        public void Update_Existing_User()
        {
            // Arrange
            var userId = _fakeDb.UserGuids[0];
            var stored = _repo.GetById(userId);
            Assert.NotNull(stored);

            var updatePayload = new UserEntity
            {
                Id = stored.Id,
                Name = "Ondra",
                PhotoUrl = stored.PhotoUrl,
                Role = stored.Role,
                IdentityOwnerId = stored.IdentityOwnerId
            };

            // Act
            var result = _repo.Update(updatePayload);

            // Assert
            var expectedUser = _fakeDb.GetUserById(userId);
            Assert.NotNull(result);
            Assert.NotNull(expectedUser);
            Assert.True(AreUsersEqual(updatePayload, expectedUser));
        }
    }
}