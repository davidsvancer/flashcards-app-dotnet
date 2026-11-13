using Flashcards.Common.Enums;
using Flashcards.Common.Models;
using Flashcards.Web.BL.Facades;
using Microsoft.AspNetCore.Components;

namespace Flashcards.Web.App.Pages.Users;

public partial class UserList : PageBase
{
    // Injections
    [Inject] private UserFacade UserFacade { get; set; } = null!;
    
    // Properties 
    private List<UserListModel> Users { get; set; } = Enumerable.Empty<UserListModel>().ToList();
    private bool IsCreating { get; set; }
    private bool IsSubmitting { get; set; }
    private string? ErrorMessage { get; set; }
    private bool IsEditing { get; set; }
    private NewUserInputs Input { get; set; } = new();
    private UserUpdateModel EditInput { get; set; }
    private int TotalPages { get; set; }
    private string _filter = String.Empty;
    private string Filter
    {
        get => _filter;
        set
        {
            _filter = value;
            _ = OnFilterChanged();
        }
    }
    
    // variables
    private UserDetailModel? _selectedUser;
    private readonly int _pageSize = 8;
    private int _currentPage = 1;
    private string _sortBy = "default";

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        await LoadData();
    }

    private async Task OnFilterChanged()
    {
        _currentPage = 1;
        await LoadData();
        StateHasChanged();
    }
    
    private async Task LoadData()
    {
        // prepare query
        var query = new UserQueryModel
        {
            SearchString = _filter                  // filter by name
        };
    
        // paging
        query.PageSize = _pageSize;
        query.Page = _currentPage;

        // sorting
        if (_sortBy == "ascending")
        {
            query.SortAsc= true;
            query.SortBy = nameof(UserDetailModel.Name);
        }
        else if (_sortBy == "descending")
        {
            query.SortAsc= false;
            query.SortBy = nameof(UserDetailModel.Name);
        }

        var result = await UserFacade.GetByQueryAsync(query);
        Users = result.Items.ToList();
        if (result.TotalCount > 0)
        {
            TotalPages = (int)Math.Ceiling((double)result.TotalCount / _pageSize);
        }
        else
        {
            TotalPages = 1;
        }
    }

    private void ShowCreateForm()
    {
        IsCreating = true;
        ErrorMessage = null;

        Input = new NewUserInputs();
    }

    private void CancelCreate()
    {
        IsCreating = false;
        ErrorMessage = null;
        Input = null!;
    }

    private async Task CreateUser()
    {
        ErrorMessage = null;
        IsSubmitting = true;

        if (string.IsNullOrWhiteSpace(Input.DisplayName) ||
            string.IsNullOrWhiteSpace(Input.Password) ||
            string.IsNullOrWhiteSpace(Input.IdentityName))
        {
            ErrorMessage = "Please fill in all required fields.";
            return;
        }

        try
        {
            var userCreateModel = new UserCreateModel()
            {
                IdentityName = Input.IdentityName,
                Name = Input.DisplayName,
                Password = Input.Password,
                PhotoUrl = string.IsNullOrWhiteSpace(Input.ProfilePictureUrl) ? null : Input.ProfilePictureUrl,
                Role = RoleType.User
            };

            await UserFacade.Create(userCreateModel);

            IsCreating = false;
            Input = null!;
            await LoadData();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error while creating user: {ex.Message}";
        }
        finally
        {
            IsSubmitting = false;
        }
    }

    private async Task DeleteUser(UserListModel user)
    {
        try
        {
            await UserFacade.Delete(user.Id);
            // Remove from the Users list immediately to reflect the change in the UI
            Users.Remove(user);
            await LoadData();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error deleting user: {ex.Message}";
        }
    }

    private record NewUserInputs
    {
        public string DisplayName { get; set; } = string.Empty;
        public string IdentityName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ProfilePictureUrl { get; set; } = string. Empty;
        public RoleType Role { get; set; } = RoleType.User;
    }
    
    // Modal view methods
    private async Task OpenUserDetails(UserListModel user)
    {
        try
        {
            _selectedUser = await UserFacade.GetByIdAsync(user.Id);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error loading user details: {ex.Message}";
        }
    }
    
    private void CloseUserDetails()
    {
        _selectedUser = null;
    }
    
    private string GetInitials(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return "?";
        return name.Trim().Substring(0, 1).ToUpper();
    }
    
    // sorting method
    private async Task HandleSortChange(ChangeEventArgs e)
    {
        _sortBy = e.Value?.ToString() ?? "default";
        _currentPage = 1; 
        await LoadData();
    }
    
    // paging methods
    private async Task NextPage()
    {
        if (_currentPage < TotalPages)
        {
            _currentPage++;
            await LoadData();
        }
    }

    private async Task PreviousPage()
    {
        if (_currentPage > 1)
        {
            _currentPage--;
            await LoadData();
        }
    }
    
    // edit methods
    private async Task UpdateUser()
    {
        ErrorMessage = null;
        IsSubmitting = true;

        if (string.IsNullOrWhiteSpace(EditInput.Name))
        {
            ErrorMessage = "Name is required.";
            IsSubmitting = false;
            return;
        }

        try
        {
            await UserFacade.Update(EditInput);

            IsEditing = false;
            await LoadData();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error updating user: {ex.Message}";
        }
        finally
        {
            IsSubmitting = false;
        }
    }
    
    private async Task  ShowEditForm(UserListModel user)
    {
        IsCreating = false;
        ErrorMessage = null;
        
        try
        {
            var userDetail = await UserFacade.GetByIdAsync(user.Id);
            if (userDetail == null) return;
            EditInput = new UserUpdateModel
            {
                Id = userDetail.Id,
                Name = userDetail.Name,
                PhotoUrl = userDetail.PhotoUrl 
            };
            
            IsEditing = true;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Could not load user data for editing: {ex.Message}";
        }
    }

    private void CancelEdit()
    {
        IsEditing = false;
        ErrorMessage = null;
    }
}