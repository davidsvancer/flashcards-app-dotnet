using Flashcards.Common.Models;
using Flashcards.Web.BL.Facades;
using Microsoft.AspNetCore.Components;

namespace Flashcards.Web.App.Pages.Play
{
    public partial class Play : PageBase
    {
        // input parameter, id of the card collection to play
        [Parameter]
        public Guid Id { get; init; }
        
        // injections
        [Inject]
        private NavigationManager NavigationManager { get; set; } = null!;
        [Inject]
        private LessonFacade LessonFacade { get; set; } = null!;
        [Inject]
        private CardCollectionFacade CardCollectionFacade { get; set; } = null!;
        
        // variables and properties
        private static CardCollectionDetailModel? CardCollection { get; set; }
        private LessonCreateModel? _newLesson;
        private LessonDetailModel? _createdLesson;
        private int _index;
        private bool _isFlipped ;
        private bool _showStats;
        private Guid _createdLessonId;
        // to track answers for each card
        private enum AnswerState { None, Correct, Wrong }

        private readonly Dictionary<Guid, AnswerState> _answers = new();
        
        protected override async Task OnInitializedAsync()
        {
            try
            {
                // load the selected card collection 
                CardCollection = await CardCollectionFacade.GetByIdAsync(Id); 
                if(CardCollection == null)
                {
                    Console.WriteLine("Card collection not found.");
                    NavigationManager.NavigateTo("/collections");
                    return;                                                                                     // prevent further execution if collection is not found
                }
                // prepare a new lesson with default values
                _newLesson = new LessonCreateModel()
                {
                    CardCollectionId = CardCollection.Id,
                    Seen = 0,
                    Wrong = 0,
                    Correct = 0,
                    Start = DateTime.Now
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to retrieve auth state: {ex.Message}");
            }
        }
        
        // check if the card with given id has been answered
        private bool IsAnswered(Guid cardId) => _answers.TryGetValue(cardId, out var s) && s != AnswerState.None;
        // get the answer state for the card with given id
        private AnswerState GetAnswerState(Guid cardId) => _answers.TryGetValue(cardId, out var s) ? s : AnswerState.None;

        // action handlers
        private async Task HandleMarkCorrectAsync(Guid cardId)
        {
            await HandleAnswerAsync(cardId, AnswerState.Correct);
            await HandleClickNext();
        }

        private async Task HandleMarkWrongAsync(Guid cardId)
        {
            await HandleAnswerAsync(cardId, AnswerState.Wrong);
            await HandleClickNext();
        }

        private Task HandleAnswerAsync(Guid cardId, AnswerState state)
        {
            if (IsAnswered(cardId)) return Task.CompletedTask;          // prevent double answering
            _answers[cardId] = state;

            if (_newLesson != null)
            {
                // update lesson statistics
                _newLesson.Seen++;
                if (state == AnswerState.Correct) _newLesson.Correct++;
                else if (state == AnswerState.Wrong) _newLesson.Wrong++;
            }
            InvokeAsync(StateHasChanged);
            return Task.CompletedTask;
        }

        private void HandleClickBack()
        {
            if (_index > 0) _index--;
        }

        private async Task HandleClickNext()
        {
            if (CardCollection != null && _index + 1 < CardCollection.Cards.Count)
            {
                _index++;
            }
            else
            {
                try
                {
                    if (_newLesson == null) return;
                    _createdLessonId = await LessonFacade.Create(_newLesson);       // create the lesson
                    await LoadCreatedLesson(_createdLessonId);                                  // load the created lesson
                    _showStats = true;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }
        
        private void HandlePlayAgain()
        {
            if (_newLesson != null)
            {
                _newLesson.Correct = 0;
                _newLesson.Wrong = 0;
                _newLesson.Seen = 0;
                _index = 0;
                _isFlipped = false;
                _showStats = false;
                _answers.Clear();  
            }
            NavigationManager.NavigateTo($"/play/{Id}");
        }
        
        private void HandleTryDifferentCollection()
        {
            NavigationManager.NavigateTo("/collections");
        }
        
        // load the created lesson 
        private async Task LoadCreatedLesson(Guid lessonId)
        {
            try
            {
                _createdLesson = await LessonFacade.GetByIdAsync(lessonId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load created lesson: {ex.Message}");
            }
        }
        
        // flip the current card
        private void FlipCard()
        {
            _isFlipped = !_isFlipped;
        }
    }
}
