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
    public class UserFacadeTests
    {
        private readonly IUserRepository _userRepoSub;
        private readonly ILessonRepository _lessonRepoSub;
        private readonly ISecurityService _securityServiceSub;
        private readonly IIdentityService _identityServiceSub;

        private readonly UserMapper _mapper;

        private readonly UserFacade _sut;

        public UserFacadeTests()
        {
            _userRepoSub = Substitute.For<IUserRepository>();
            _lessonRepoSub = Substitute.For<ILessonRepository>();
            _securityServiceSub = Substitute.For<ISecurityService>();
            _identityServiceSub = Substitute.For<IIdentityService>();

            _mapper = new UserMapper();

            _sut = new UserFacade(
                _userRepoSub,
                _lessonRepoSub,
                _securityServiceSub,
                _identityServiceSub,
                _mapper
            );
        }

        [Fact]
        public void Create_ShouldThrowForbidden_WhenUserIsNotAdmin()
        {
            // Arrange
            var model = new UserCreateModel
            {
                IdentityName = "hacker", 
                Role = RoleType.User ,
                Name = "Anonymous123",
                Password = "Pass123"
            };

            _securityServiceSub
                .When(x => x.ThrowIfNotAdmin())
                .Do(x => throw new UnauthorizedAccessException("Admin only"));

            // Act
            Action act = () => _sut.Create(model);

            // Assert
            Assert.Throws<UnauthorizedAccessException>(act);

            // Verify we never called IdentityService or Repository
            _identityServiceSub.DidNotReceive().CreateIdentityAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>());
            _userRepoSub.DidNotReceive().Insert(Arg.Any<UserEntity>());
        }

        [Fact]
        public void Create_ShouldThrowInvalidOperation_WhenIdentityCreationFails()
        {
            // Arrange
            var model = new UserCreateModel
            {
                IdentityName = "newuser",
                Password = "123",
                Role = RoleType.User,
                Name = "New User"
            };

            // Simulate Identity Service returning failure (empty string)
            _identityServiceSub.CreateIdentityAsync(model.IdentityName, model.Password, "User")
                .Returns(string.Empty);

            // Act
            Action act = () => _sut.Create(model);

            // Assert
            Assert.Throws<InvalidOperationException>(act); // failed to create identity

            // Verify we did NOT try to insert a broken user into the DB
            _userRepoSub.DidNotReceive().Insert(Arg.Any<UserEntity>());
        }

        [Fact]
        public void Create_ShouldCreateIdentityAndInsertUser_WhenDataIsValid()
        {
            // Arrange
            var model = new UserCreateModel
            {
                IdentityName = "john_doe",
                Password = "SecurePassword123!",
                Role = RoleType.User,
                Name = "John Doe"
            };

            var expectedIdentityId = "auth0|123456";
            var expectedUserId = Guid.NewGuid();

            //  Setup Identity Service to return a valid ID
            _identityServiceSub
                .CreateIdentityAsync(model.IdentityName, model.Password, "User")
                .Returns(expectedIdentityId);

            // Setup Repository to return a new Guid
            _userRepoSub
                .Insert(Arg.Any<UserEntity>())
                .Returns(expectedUserId);

            // Act
            var result = _sut.Create(model);

            // Assert
            Assert.Equal(expectedUserId, result);

            // Verify dependencies were called exactly once
            _securityServiceSub.Received(1).ThrowIfNotAdmin();

            // VERIFY DATA INTEGRITY:
            _userRepoSub.Received(1).Insert(Arg.Is<UserEntity>(e =>
                e.IdentityOwnerId == expectedIdentityId &&
                e.Role == RoleType.User &&
                e.Name == "John Doe"
            ));
        }

        [Fact]
        void Delete_ShouldDoNothing_WhenAlreadyDeletedUserIsDeleted()
        {
            // Arrange
            var targetGuid = Guid.NewGuid();
            var deletedUser = new UserEntity()
            {
                Id = targetGuid,
                IdentityOwnerId = string.Empty,
                IsDeleted = true,
                Name = "I was deleted",
                Role = RoleType.User
            };
            _userRepoSub.GetById(targetGuid).Returns(deletedUser);

            // Act
            _sut.Delete(targetGuid);

            // Assert
            _userRepoSub.DidNotReceiveWithAnyArgs().Remove(targetGuid);
            _identityServiceSub.DidNotReceiveWithAnyArgs().DeleteIdentityAsync(deletedUser.IdentityOwnerId);
        }
    }
}

