using Flashcards.Common.Models;
using Flashcards.IdentityProvider.BL.Facades;
using Flashcards.IdentityProvider.BL.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Flashcards.IdentityProvider.App.Endpoints;

public static class UserEndpoints
{
    public static IEndpointRouteBuilder UseUserEndpoints(this IEndpointRouteBuilder endpointRouteBuilder)
    {
        var userEndpoints = endpointRouteBuilder.MapGroup("user");

        userEndpoints.MapGet("search",
            async (
                    IAppUserFacade appUserFacade,
                    string searchString)
                => await appUserFacade.SearchAsync(searchString));

        userEndpoints.MapPost("",
            async Task<Results<Created<string>, BadRequest, BadRequest<string>>> (
                    IAppUserFacade appUserFacade,
                    [FromBody] AppUserCreateModel appUser)
                =>
            {
                try
                {
                    var userId = await appUserFacade.CreateAppUserAsync(appUser);
                    if (userId is not null)
                    {
                        return TypedResults.Created($"/user/{userId}", userId);
                    }

                    return TypedResults.BadRequest();
                }
                catch (ArgumentException e)
                {
                    return TypedResults.BadRequest(e.Message);
                }
                catch (InvalidOperationException e)
                {
                    return TypedResults.BadRequest(e.Message);
                }
            });

        userEndpoints.MapDelete("{id}",
            async Task<Results<Ok, NotFound, BadRequest<string>>> (
                    IAppUserFacade appUserFacade,
                    string id)
                =>
            {
                try
                {
                    var result = await appUserFacade.DeleteAppUserAsync(id);
                    if (result)
                    {
                        return TypedResults.Ok();
                    }
                    return TypedResults.NotFound();
                }
                catch (Exception e)
                {
                    return TypedResults.BadRequest(e.Message);
                }
            });

        return endpointRouteBuilder;
    }
}