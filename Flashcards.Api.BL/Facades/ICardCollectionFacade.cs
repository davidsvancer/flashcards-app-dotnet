using Flashcards.Common.BL.Facades;
using Flashcards.Common.Models;

namespace Flashcards.Api.BL.Facades
{
    public interface ICardCollectionFacade : IFacade
    {
        List<CardCollectionListModel> GetAll();
        PagedResponse<CardCollectionListModel> GetByQuery(CardCollectionQueryModel query);
        CardCollectionDetailModel? GetById(Guid id);
        Guid CreateOrUpdate(CardCollectionDetailModel model);
        Guid Create(CardCollectionCreateModel model);
        Guid? Update(CardCollectionDetailModel model);
        void Delete(Guid id);
    }
}
