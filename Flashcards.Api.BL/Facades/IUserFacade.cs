using Flashcards.Common.BL.Facades;
using Flashcards.Common.Models;

namespace Flashcards.Api.BL.Facades
{
    public interface IUserFacade : IFacade
    {
        List<UserListModel> GetAll();
        PagedResponse<UserListModel> GetByQuery(UserQueryModel query);
        UserDetailModel? GetById(Guid id);
        UserDetailModel? GetByIdentityId(string identityId);
        Guid Create(UserCreateModel user);
        Guid? Update(UserUpdateModel user);
        void Delete(Guid id);
    }
}
