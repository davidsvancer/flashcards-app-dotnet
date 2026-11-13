using Flashcards.Common.Models;
using Flashcards.Web.BL.Facades;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace Flashcards.Web.App.Pages.Statistics
{
    public partial class StatisticsPage : PageBase
    {
        // injections
        [Inject]
        private AuthenticationStateProvider AuthStateProvider { get; set; } = null!;
        [Inject]
        private UserFacade UserFacade { get; set; } = null!;
        [Inject]
        private LessonFacade LessonFacade { get; set; } = null!;
        [Inject]
        private NavigationManager NavigationManager { get; set; } = null!;
        
        // variables and properties
        private List<LessonListModel>? _lessons;
        private ICollection<LessonListModel> _allLessons = new List<LessonListModel>();
        private UserDetailModel? _user; 
        private LessonListModel? _selectedLesson;
        private bool _isLoading = true;
        private string? _userId;
        private int _pageNumber = 1;
        private readonly  int _pageSize = 6;
        private int _totalPages = 1;
        private long _totalItems;
        private string _sortOrder = "descending"; 
        private string SortOrder
        {
            get => _sortOrder;
            set
            {
                _sortOrder = value;
                _pageNumber = 1; 
                _ = LoadData(); 
            }
        }
        private long TotalCardsSeen => _lessons?.Sum(l => l.Seen) ?? 0;
        
        private double GlobalAccuracy()
        {
            if ( _allLessons.Count == 0) return 0;
            var totalCorrect = _allLessons.Sum(l => (double)l.Correct);
            var totalWrong = _allLessons.Sum(l => (double)l.Wrong);
            var totalAnswers = totalCorrect + totalWrong;
            return totalAnswers == 0 ? 0 : (totalCorrect / totalAnswers) * 100;
        }
    
        protected override async Task OnInitializedAsync()
        {
            try
            {
                // get current user identity
                var authState = await AuthStateProvider.GetAuthenticationStateAsync();
                var user = authState.User;
                if (user.Identity?.IsAuthenticated == true)
                {
                    _userId = user.FindFirst("sub")?.Value;
                }
                
                await LoadData();
                await base.OnInitializedAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading stats: {ex.Message}");
            }
            finally
            {
                _isLoading = false;
            }
        }

        private async Task LoadData()
        {
            if (string.IsNullOrEmpty(_userId))
            {
                _lessons = new List<LessonListModel>();
                return;
            }

            _isLoading = true;
            StateHasChanged();

            try 
            {
                // get current user details
                if (_user == null) 
                {
                    _user = await UserFacade.GetByIdentityIdAsync(_userId);
                }

                // get all lessons for user
                _allLessons = await LessonFacade.GetAllAsync();
                
                // prepare query
                var query = new LessonQueryModel
                {
                    UserId = _user.Id,
                    Page = _pageNumber,
                    PageSize = _pageSize,
                };

                // sorting
                if (_sortOrder == "ascending")
                {
                    query.SortAsc = true;
                    query.SortBy = nameof(LessonDetailModel.Start);
                }
                else if (_sortOrder == "descending")
                {
                    query.SortAsc = false;
                    query.SortBy = nameof(LessonDetailModel.Start);
                }
                
                //  get lessons and pages info
                var lessonsPagedResult = await LessonFacade.GetByQueryAsync(query);
                _lessons = lessonsPagedResult.Items.ToList();
                _totalItems = lessonsPagedResult.TotalCount;
                _totalPages = (int)Math.Ceiling((double)_totalItems / _pageSize);
                if (_totalPages < 1) _totalPages = 1;
            }
            finally 
            {
                _isLoading = false;
                StateHasChanged();
            }
        }
        
        // Helpers methods
        private async Task NextPage()
        {
            if (_pageNumber < _totalPages)
            {
                _pageNumber++;
                await LoadData();
            }
        }

        private async Task PreviousPage()
        {
            if (_pageNumber > 1)
            {
                _pageNumber--;
                await LoadData();
            }
        }
        
        private static double CalculateAccuracy(double correct, double wrong)
        {
            var total = correct + wrong;
            if (total == 0) return 0;
            return (correct / total) * 100;
        }

        private static string GetAccuracyColor(double accuracy) => accuracy switch
        {
            >= 80 => "text-green-600",
            >= 50 => "text-yellow-600",
            _ => "text-red-600"
        };

        private static string GetInitials(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return "?";
            return name.Trim().Substring(0, 1).ToUpper();
        }
        
        // helper function for modal window
        private void OpenLessonDetails(LessonListModel lesson)
        {
            _selectedLesson = lesson;
        }
        
        private void CloseLessonDetails()
        {
            _selectedLesson = null;
        }

        private void HandlePlayAgain()
        {
            if (_selectedLesson != null)
            {
                NavigationManager.NavigateTo($"/play/{_selectedLesson.CardCollectionId}");
            }
        }
    }
}