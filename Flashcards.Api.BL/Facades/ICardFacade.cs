using Flashcards.Api.DAL.Common.Entities;
using Flashcards.Common.BL.Facades;
using Flashcards.Common.Models;

namespace Flashcards.Api.BL.Facades
{
    public interface ICardFacade : IFacade
    {
        List<CardListModel> GetAll();
        PagedResponse<CardListModel> GetByQuery(CardQueryModel query);
        CardDetailModel? GetById(Guid id);
        Guid Create(CardCreateModel card);
        Guid CreateOrUpdate(CardDetailModel card);
        Guid? Update(CardDetailModel card);
        void Delete(Guid id);
    }
}
