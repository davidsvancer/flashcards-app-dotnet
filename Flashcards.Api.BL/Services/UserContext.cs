using Flashcards.Common.Enums;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Flashcards.Api.BL.Services.Interfaces;

namespace Flashcards.Api.BL.Services
{
    public class UserContext(IHttpContextAccessor httpContext) : IUserContext
    {
        public string CurrentIdentityId =>
            httpContext.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new UnauthorizedAccessException("User is not authenticated");

        public bool IsAdmin =>
            httpContext.HttpContext?.User.IsInRole(nameof(RoleType.Admin)) ?? false;
    }
}
