namespace Flashcards.Api.BL.Services.Interfaces
{
    public interface IUserContext
    {
        string CurrentIdentityId { get; }
        bool IsAdmin { get; }
    }
}
