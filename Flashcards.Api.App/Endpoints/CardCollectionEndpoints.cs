using Flashcards.Api.BL.Facades;
using Flashcards.Common.Models;
using Flashcards.Common.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Options;

namespace Flashcards.Api.App.Endpoints;

public class CardCollectionEndpoints(IOptions<IdentityOptions> identityOptions) : EndpointsBase
{
    public override IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpointRouteBuilder)
    {
        var cardCollectionEndpoints = endpointRouteBuilder
            .MapGroup("cardCollection")
            .WithTags("cardCollection");

        cardCollectionEndpoints.MapGet("", 
            (ICardCollectionFacade cardCollectionFacade) =>
            {
                return TypedResults.Ok(cardCollectionFacade.GetAll());
            });

        cardCollectionEndpoints.MapPost("query", 
            (CardCollectionQueryModel query, ICardCollectionFacade cardCollectionFacade) =>
            {
                return TypedResults.Ok(cardCollectionFacade.GetByQuery(query));
            });

        cardCollectionEndpoints.MapGet("{id:guid}", Results<Ok<CardCollectionDetailModel>, NotFound<string>> 
            (Guid id, ICardCollectionFacade cardCollectionFacade) =>
            {
                return cardCollectionFacade.GetById(id) is { } cardCollection
                    ? TypedResults.Ok(cardCollection)
                    : TypedResults.NotFound("This card collection was not found!");
            });

        var cardCollectionModifyingEndpoints = cardCollectionEndpoints.MapGroup("");
        if (identityOptions.Value.IsIdentityEnabled)
        {
            cardCollectionModifyingEndpoints.RequireAuthorization();
        }

        cardCollectionModifyingEndpoints.MapPost("", 
            (CardCollectionCreateModel cardCollection, ICardCollectionFacade cardCollectionFacade) =>
            {
                var id = cardCollectionFacade.Create(cardCollection);
                return TypedResults.Ok(id);
            });

        cardCollectionModifyingEndpoints.MapPut("", Results<Ok<Guid?>, ForbidHttpResult> 
            (CardCollectionDetailModel cardCollection, ICardCollectionFacade cardCollectionFacade) =>
            {
                try
                {
                    return TypedResults.Ok(cardCollectionFacade.Update(cardCollection));
                }
                catch (UnauthorizedAccessException)
                {
                    return TypedResults.Forbid();
                }
            });

        cardCollectionEndpoints.MapPost("upsert", 
            (CardCollectionDetailModel cardCollection, ICardCollectionFacade cardCollectionFacade) =>
            {
                var id = cardCollectionFacade.CreateOrUpdate(cardCollection);
                return TypedResults.Ok(id);
            });

        cardCollectionModifyingEndpoints.MapDelete("{id:guid}", Results<Ok, ForbidHttpResult> 
            (Guid id, ICardCollectionFacade cardCollectionFacade) =>
            {
                try
                {
                    cardCollectionFacade.Delete(id);
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
