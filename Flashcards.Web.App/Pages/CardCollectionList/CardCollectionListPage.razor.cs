using Flashcards.Common.Models;
using Flashcards.Web.BL.Facades;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace Flashcards.Web.App.Pages.CardCollectionList
{
    public partial class CardCollectionListPage : PageBase
    {
        // Injections
        [Inject] private CardCollectionFacade CardCollectionFacade { get; set; } = null!;
        [Inject] private UserFacade UserFacade { get; set; } = null!;
        [Inject] private NavigationManager NavigationManager { get; set; } = null!;
        [Inject] private AuthenticationStateProvider AuthStateProvider { get; set; } = null!;

        // enum
        private enum ViewMode
        {
            MyCollections,
            AllCollections
        }

        // variables and properties
        private bool _showCreateForm;
        private bool _onlyAvailable;
        private int _currentPage = 1;
        private readonly int _pageSize = 9;
        private ViewMode _currentView = ViewMode.MyCollections;
        private int TotalPages { get; set; }
        private string? _userId;
        private bool IsLoading { get; set; } = false;
        private string _sortBy = "default";
        private ICollection<CardCollectionListModel> CardCollectionsAll { get; set; } = new List<CardCollectionListModel>();
        private ICollection<CardCollectionListModel> CardCollectionsMine { get; set; } = new List<CardCollectionListModel>();
        private CardCollectionDetailModel? _selectedCollection;
        private CardCollectionListModel? _selectedCollectionListModel;
        private string _filterByTitle = string.Empty;

        private string FilterByTitle
        {
            get => _filterByTitle;
            set
            {
                _filterByTitle = value;
                _ = OnFilterChanged();
            }
        }

        private bool OnlyAvailable
        {
            get => _onlyAvailable;
            set
            {
                _onlyAvailable = value;
                _ = OnFilterChanged();
            }
        }

        private CardCollectionDetailModel _model = new CardCollectionDetailModel
        {
            Id = Guid.NewGuid(),
            Title = string.Empty,
            AnswerStart = DateTime.Now,
            AnswerEnd = DateTime.Now.AddDays(1),
            Cards = new List<CardDetailModel>()
        };

        protected override async Task OnInitializedAsync()
        {
            try
            {
                await base.OnInitializedAsync();
                // get current user identity
                var authState = await AuthStateProvider.GetAuthenticationStateAsync();
                var user = authState.User;
                if (user.Identity?.IsAuthenticated == true)
                {
                    _userId = user.FindFirst("sub")?.Value;
                }
                await LoadData();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to retrieve auth state: {ex.Message}");
            }
        }

        private async Task LoadData()
        {
            if (string.IsNullOrEmpty(_userId))
            {
                // No user logged in, show empty collection
                CardCollectionsMine = Array.Empty<CardCollectionListModel>();
                return;
            }

            // get currently signed user
            var currentUser = await UserFacade.GetByIdentityIdAsync(_userId);

            // create query model 
            var query = new CardCollectionQueryModel
            {
                SearchString = FilterByTitle, // by title
                ShowCurrentlyAvailableOnly = OnlyAvailable // only active collections
            };

            // set pagination parameters
            query.PageSize = _pageSize; // how many items per page
            query.Page = _currentPage;

            // sorting
            if (_sortBy == "ascending")
            {
                query.SortAsc = true;
                query.SortBy = nameof(CardCollectionDetailModel.Title);
            }
            else if (_sortBy == "descending")
            {
                query.SortAsc = false;
                query.SortBy = nameof(CardCollectionDetailModel.Title);
            }

            if (_currentView == ViewMode.MyCollections)
            {
                query.OwnerId = currentUser.Id;     // only collections of current user
                var cardCollectionsOfCurrentUser = await CardCollectionFacade.GetByQueryAsync(query);
                CardCollectionsMine = cardCollectionsOfCurrentUser.Items;
                if (cardCollectionsOfCurrentUser.TotalCount > 0)
                {
                    TotalPages = (int)Math.Ceiling((double)cardCollectionsOfCurrentUser.TotalCount / _pageSize);
                }
                else
                {
                    TotalPages = 1;
                }
            }
            else
            {
                query.ExcludeOwnerId = currentUser.Id;      // all collections except those of current user
                var cardCollectionsExcludingCurrentUser = await CardCollectionFacade.GetByQueryAsync(query);
                CardCollectionsAll = cardCollectionsExcludingCurrentUser.Items;
                if (cardCollectionsExcludingCurrentUser.TotalCount > 0)
                {
                    TotalPages = (int)Math.Ceiling((double)cardCollectionsExcludingCurrentUser.TotalCount / _pageSize);
                }
                else
                {
                    TotalPages = 1;
                }
            }
        }

        // navigation handlers
        private Task OnEditCollection(CardCollectionListModel collection)
        {
            NavigationManager.NavigateTo($"collections/edit/{collection.Id}");
            return Task.CompletedTask;
        }

        private void HandlePlayCollection(CardCollectionListModel collection)
        {
            NavigationManager.NavigateTo($"/play/{collection.Id}");
        }

        // action handlers
        private async Task OnDeleteCollection(CardCollectionListModel collection)
        {
            try
            {
                await CardCollectionFacade.DeleteAsync(collection.Id);
                await LoadData();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to delete collection: {ex.Message}");
            }
        }

        private async Task HandleSubmit(CardCollectionDetailModel model)
        {
            try
            {
                CardCollectionDetailModel newCollection = new CardCollectionDetailModel
                {
                    Id = Guid.NewGuid(),
                    Title = model.Title,
                    AnswerStart = model.AnswerStart,
                    AnswerEnd = model.AnswerEnd,
                    Cards = model.Cards
                };

                _showCreateForm = false;
                await CardCollectionFacade.UpsertAsync(newCollection);
                await LoadData();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to create collection: {ex.Message}");
            }
        }

        private async Task OnFilterChanged()
        {
            await LoadData();
            StateHasChanged();
        }

        private void HandleCancel()
        {
            _showCreateForm = false;
        }

        private void HandleOpenForm()
        {
            _showCreateForm = true;
        }

        // checks if current date is within answer period
        private static bool IsCollectionActive(CardCollectionListModel collection)
        {
            var now = DateTime.Now;
            var startUtc = collection.AnswerStart;
            var endUtc = collection.AnswerEnd;
            return now >= startUtc && now <= endUtc;
        }

        private async Task SwitchView(ViewMode mode)
        {
            if (_currentView == mode) return;
            _currentView = mode;
            _currentPage = 1;                               // Reset to page 1 on view change 
            _showCreateForm = false;                // Reset creating form
            await LoadData();
        }

        private async Task HandleFilterChange()
        {
            _currentPage = 1;
            await LoadData();
        }

        private async Task ChangePage(int newPage)
        {
            if (newPage < 1 || newPage > TotalPages) return;
            _currentPage = newPage;
            await LoadData();
        }

        // modal window 
        private async Task OpenDetailsModal(CardCollectionListModel collection)
        {
            try
            {
                _selectedCollectionListModel = collection;
                _selectedCollection = await  CardCollectionFacade.GetByIdAsync(collection.Id);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                _selectedCollection = null;
            }
        }

        private void CloseDetailsModal()
        {
            _selectedCollection = null;
        }
    }
}
