using Flashcards.Common.Models;
using Flashcards.Common.Enums;
using Flashcards.Web.BL.Facades;
using Microsoft.AspNetCore.Components;

namespace Flashcards.Web.App.Pages.CardCollectionEdit
{
    public partial class CardCollectionEditPage : PageBase
    {
        // input parameter
        [Parameter] public Guid Id { get; set; }
        
        // injections
        [Inject] private CardCollectionFacade CardCollectionFacade { get; set; } = null!;
        [Inject] private CardFacade CardFacade { get; set; } = null!;
        [Inject] private NavigationManager NavigationManager { get; set; } = null!;

        // variables and properties
        private CardCollectionDetailModel? CardCollection { get; set; }
        private CardDetailModel? NewCard { get; set; }

         protected override async Task OnInitializedAsync()
         {
             try
             {
                 await LoadData();
                 await base.OnInitializedAsync();
             }
             catch (Exception ex)
             {
                 Console.WriteLine(ex);
             }
        }
         
         private async Task LoadData()
         {
             // load cards by collection id
             CardCollection = await CardCollectionFacade.GetByIdAsync(Id);
             // initialize new card with defaults
             NewCard = SetCardDefaults();
         }
         
         // CRUD methods
         private async Task SaveCollection()
         {
             try
             {
                 if (CardCollection != null)
                 {
                     await CardCollectionFacade.UpsertAsync(CardCollection);   
                 }
                 NavigationManager.NavigateTo("/collections");
             }
             catch (Exception exception)
             {
                 Console.WriteLine(exception);
             }
         }
         
         private async Task DeleteCard(CardDetailModel card)
         {
             if (CardCollection != null)
             {
                 CardCollection.Cards.Remove(card);
                 await CardCollectionFacade.UpsertAsync(CardCollection);
                 await LoadData();  
             }
         }
         
        private async Task HandleAddNewCard()
        {
            try
            {
                if (CardCollection != null && NewCard != null)
                {
                    CardCollection.Cards.Add(NewCard);
                    await CardCollectionFacade.UpsertAsync(CardCollection);
                    NewCard = SetCardDefaults();   
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
            finally
            {
                await LoadData();
            }
        }
        
        // set default values for new card
        private CardDetailModel SetCardDefaults()
        {
            return new CardDetailModel
            {
                Id = Guid.NewGuid(),
                Description = string.Empty,
                QuestionContent = string.Empty,
                AnswerContent = string.Empty,
                QuestionType = ContentType.Text,
                AnswerType = ContentType.Text,
                DateCreated = DateTime.UtcNow
            };
        }
    }
}
