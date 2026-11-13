using Flashcards.Common.Models;
using Flashcards.Web.BL.Api;

namespace Flashcards.Web.BL.Facades
{
    public class CardFacade(ICardApiClient apiClient)
    {
        public async Task<Guid> UpsertAsync(CardDetailModel model)
        {
            var id = await apiClient.UpsertAsync(model);
            return id;
        }

        public async Task DeleteAsync(Guid id)
        {
            await apiClient.CardDeleteAsync(id);
        }

        public async Task<List<CardListModel>> GetAllAsync()
        {
            var cards = await apiClient.CardGetAsync();
            return cards.ToList();
        }

        public async Task<PagedResponse<CardListModel>> GetByQueryAsync(CardQueryModel query)
        {
            var cards = await apiClient.QueryAsync(query);
            return cards;
        }

        public async Task<CardDetailModel?> GetByIdAsync(Guid id)
        {
            try
            {
                var result = await apiClient.CardGetAsync(id);
                return result;
            }
            catch
            {
                return null;
            }
        }
    }
}