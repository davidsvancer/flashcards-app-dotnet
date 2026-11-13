namespace Flashcards.Common.Options;

public class IdentityOptions
{
    public bool IsIdentityEnabled { get; set; }
    public required string Authority { get; set; }
    public IList<string> DefaultScopes { get; set; } = [];
    public string? BackendClientId { get; set; }
    public string? BackendClientSecret { get; set; }
    public string? UserManagementScope { get; set; }
}