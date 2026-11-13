namespace Flashcards.Api.BL.Services.Interfaces
{
    public interface IIdentityService
    {
        Task<string> CreateIdentityAsync(string username, string password, string role);
        Task DeleteIdentityAsync(string userId);
    }
}
