using Flashcards.Api.BL.Services;
using Flashcards.Api.BL.Services.Interfaces;
using Flashcards.Api.DAL.Common.Entities;
using Flashcards.Api.DAL.Common.Entities.Interfaces;
using Flashcards.Api.DAL.Common.Repositories;
using NSubstitute;
using Flashcards.Common.Enums;

namespace Flashcards.Api.App.Tests.Services
{
    public class SecurityServiceTests
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserContext _userContext;
        private readonly SecurityService _sut;

        public SecurityServiceTests()
        {
            _userRepository = Substitute.For<IUserRepository>();
            _userContext = Substitute.For<IUserContext>();
            _sut = new SecurityService(_userRepository, _userContext);
        }

        [Fact]
        public void ThrowIfNotAdmin_WhenUserIsAdmin_DoesNotThrow()
        {
            // Arrange
            _userContext.IsAdmin.Returns(true);

            // Act
            var exception = Record.Exception(() => _sut.ThrowIfNotAdmin());

            // Assert
            Assert.Null(exception);
        }

        [Fact]
        public void ThrowIfNotAdmin_WhenUserIsNotAdmin_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            _userContext.IsAdmin.Returns(false);
            _userContext.CurrentIdentityId.Returns("some_user");

            // Act & Assert
            Assert.Throws<UnauthorizedAccessException>(() => _sut.ThrowIfNotAdmin());
        }

        [Fact]
        public void ThrowIfNotOwnerAndAdmin_WhenUserIsAdmin_DoesNotThrowAndDoesNotCheckRepository()
        {
            // Arrange
            _userContext.IsAdmin.Returns(true);
            var resource = Substitute.For<IOwnedEntity>();

            // Act
            _sut.ThrowIfNotOwnerAndAdmin(resource);

            // Assert
            // Ensure we didnt even try to access the Storage because Admin bypasses everything
            _userRepository.DidNotReceiveWithAnyArgs().GetByIdentityId(default!);
        }

        [Fact]
        public void ThrowIfNotOwnerAndAdmin_WhenUserIsOwner_DoesNotThrow()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var identityId = "owner_identity";

            _userContext.IsAdmin.Returns(false);
            _userContext.CurrentIdentityId.Returns(identityId);

            // Mock the repository to return a valid user matching the identity
            var userEntity = new UserEntity { Id = userId, IdentityOwnerId = identityId, Name = default, Role = RoleType.User};
            _userRepository.GetByIdentityId(identityId).Returns(userEntity);

            // Mock the resource to be owned by this user
            var resource = Substitute.For<IOwnedEntity>();
            resource.OwnerId.Returns(userId);

            // Act
            var exception = Record.Exception(() => _sut.ThrowIfNotOwnerAndAdmin(resource));

            // Assert
            Assert.Null(exception);
        }

        [Fact]
        public void ThrowIfNotOwnerAndAdmin_WhenUserIsNotOwner_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var myUserId = Guid.NewGuid();
            var otherOwnerId = Guid.NewGuid();
            var identityId = "my_identity";

            _userContext.IsAdmin.Returns(false);
            _userContext.CurrentIdentityId.Returns(identityId);

            // Repository returns ME
            var userEntity = new UserEntity { Id = myUserId , IdentityOwnerId = identityId, Name = default, Role = RoleType.User};
            _userRepository.GetByIdentityId(identityId).Returns(userEntity);

            // Resource is owned by SOMEONE ELSE
            var resource = Substitute.For<IOwnedEntity>();
            resource.OwnerId.Returns(otherOwnerId);

            // Act & Assert
            var ex = Assert.Throws<UnauthorizedAccessException>(() => _sut.ThrowIfNotOwnerAndAdmin(resource));
            Assert.Equal("Only an owner of this entity or admin can perform this operation", ex.Message);
        }

        [Fact]
        public void ThrowIfNotOwnerAndAdmin_WhenUserDoesNotExistInDb_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            _userContext.IsAdmin.Returns(false);
            _userContext.CurrentIdentityId.Returns("ghost_user");

            // Repository returns NULL (User not found)
            _userRepository.GetByIdentityId("ghost_user").Returns((UserEntity?)null);

            var resource = Substitute.For<IOwnedEntity>();

            // Act & Assert
            var ex = Assert.Throws<UnauthorizedAccessException>(() => _sut.ThrowIfNotOwnerAndAdmin(resource));
            Assert.Contains("There is no App User account assigned", ex.Message);
        }

        [Fact]
        public void ThrowIfUnauthorizedUserUpdate_WhenUserIsAdmin_DoesNotThrow()
        {
            // Arrange
            _userContext.IsAdmin.Returns(true);

            // Act
            var exception = Record.Exception(() => _sut.ThrowIfUnauthorizedUserUpdate(Guid.NewGuid()));

            // Assert
            Assert.Null(exception);
        }

        [Fact]
        public void ThrowIfUnauthorizedUserUpdate_WhenUpdatingSelf_DoesNotThrow()
        {
            // Arrange
            var myUserId = Guid.NewGuid();
            var identityId = "my_identity";

            _userContext.IsAdmin.Returns(false);
            _userContext.CurrentIdentityId.Returns(identityId);

            // DB returns correct user
            _userRepository.GetByIdentityId(identityId).Returns(new UserEntity { Id = myUserId, IdentityOwnerId = identityId, Name = default, Role = RoleType.User});

            // Act
            var exception = Record.Exception(() => _sut.ThrowIfUnauthorizedUserUpdate(myUserId));

            // Assert
            Assert.Null(exception);
        }

        [Fact]
        public void ThrowIfUnauthorizedUserUpdate_WhenUpdatingSomeoneElse_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var myUserId = Guid.NewGuid();
            var targetUserId = Guid.NewGuid();
            var identityId = "my_identity";

            _userContext.IsAdmin.Returns(false);
            _userContext.CurrentIdentityId.Returns(identityId);

            // Storage returns ME
            _userRepository.GetByIdentityId(identityId).Returns(new UserEntity { Id = myUserId, IdentityOwnerId = identityId, Name = default, Role = RoleType.User });

            // Act & Assert
            var ex = Assert.Throws<UnauthorizedAccessException>(() => _sut.ThrowIfUnauthorizedUserUpdate(targetUserId));
            Assert.Equal("Cannot edit another users profile.", ex.Message);
        }
    }
}