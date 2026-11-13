using Flashcards.Api.BL.Facades;
using Flashcards.Common.Models;
using Flashcards.Common.Options;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Options;

namespace Flashcards.Api.App.Endpoints;

public class UserEndpoints(IOptions<IdentityOptions> identityOptions) : EndpointsBase
{
    public override IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpointRouteBuilder)
    {
        var userEndpoints = endpointRouteBuilder
            .MapGroup("user")
            .WithTags("user");

        userEndpoints.MapGet("", 
            (IUserFacade userFacade) =>
            {
                return TypedResults.Ok(userFacade.GetAll());
            });

        userEndpoints.MapPost("query", 
            (UserQueryModel query, IUserFacade userFacade) =>
            {
                return TypedResults.Ok(userFacade.GetByQuery(query));
            });

        userEndpoints.MapGet("{id:guid}", Results<Ok<UserDetailModel>, NotFound<string>> 
            (Guid id, IUserFacade userFacade) =>
            {
                return userFacade.GetById(id) is { } user
                    ? TypedResults.Ok(user)
                    : TypedResults.NotFound("This user was not found!");
            });

        userEndpoints.MapGet("identity/{id}", Results<Ok<UserDetailModel>, NotFound<string>>
            (string id, IUserFacade userFacade) =>
            {
                return userFacade.GetByIdentityId(id) is { } user
                    ? TypedResults.Ok(user)
                    : TypedResults.NotFound("This user was not found!");
            });
        
        var userModifyingEndpoints = userEndpoints.MapGroup("");
        if (identityOptions.Value.IsIdentityEnabled)
        {
            userModifyingEndpoints.RequireAuthorization();
        }

        userModifyingEndpoints.MapPost("",
            (UserCreateModel user, IUserFacade userFacade) =>
            {
                var id = userFacade.Create(user);
                return TypedResults.Ok(id);
            });

        userModifyingEndpoints.MapPut("", Results<Ok<Guid?>, ForbidHttpResult>
            (UserUpdateModel user, IUserFacade userFacade) =>
            {
                try
                {
                    return TypedResults.Ok(userFacade.Update(user));
                }
                catch (UnauthorizedAccessException)
                {
                    return TypedResults.Forbid();
                }
            });

        userModifyingEndpoints.MapDelete("{id:guid}", Results<Ok, ForbidHttpResult> 
            (Guid id, IUserFacade userFacade) =>
            {
                try
                {
                    userFacade.Delete(id);
                    return TypedResults.Ok();
                }
                catch (UnauthorizedAccessException)
                {
                    return TypedResults.Forbid();
                }
            });
        
        return endpointRouteBuilder;
    }
}
