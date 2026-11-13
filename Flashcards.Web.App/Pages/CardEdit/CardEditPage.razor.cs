using Flashcards.Common.Models;
using Flashcards.Web.BL.Facades;
using Microsoft.AspNetCore.Components;

namespace Flashcards.Web.App.Pages.CardEdit
{
    public partial class CardEditPage : PageBase
    {
        [Parameter]
        public Guid Id { get; set; }

        [Inject]
        private CardFacade CardFacade { get; set; } = null!;

        [Inject]
        private NavigationManager NavigationManager { get; set; } = null!;

        private CardDetailModel? _card;

        protected override async Task OnInitializedAsync()
        {
            await LoadData();
        }

        private async Task LoadData()
        {
            try
            {
                _card = await CardFacade.GetByIdAsync(Id);

                if (_card == null)
                {
                    NavigationManager.NavigateTo("/cards");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading card: {ex.Message}");
            }
        }

        private async Task SaveCard()
        {
            if (_card == null) return;

            try
            {
                await CardFacade.UpsertAsync(_card);
                NavigationManager.NavigateTo("/cards");
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Error saving card: {exception.Message}");
            }
        }
    }
}