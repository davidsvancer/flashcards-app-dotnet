using Flashcards.Api.BL.Facades;
using Flashcards.Common.Models;
using Flashcards.Common.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Options;

namespace Flashcards.Api.App.Endpoints;

public class CardEndpoints(IOptions<IdentityOptions> identityOptions) : EndpointsBase
{
    public override IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpointRouteBuilder)
    {
        var cardEndpoints = endpointRouteBuilder
            .MapGroup("card")
            .WithTags("card");

        cardEndpoints.MapGet("",
            (ICardFacade cardFacade) =>
            {
                return TypedResults.Ok(cardFacade.GetAll());
            });

        cardEndpoints.MapPost("query",
            (CardQueryModel query, ICardFacade cardFacade) =>
            {
                return TypedResults.Ok(cardFacade.GetByQuery(query));
            });
        

        cardEndpoints.MapGet("{id:guid}", Results<Ok<CardDetailModel>, NotFound<string>>
            (Guid id, ICardFacade cardFacade) =>
            {
                return cardFacade.GetById(id) is { } card
                    ? TypedResults.Ok(card)
                    : TypedResults.NotFound("This card was not found!");
            });

        var cardModifyingEndpoints = cardEndpoints.MapGroup("");
        if (identityOptions.Value.IsIdentityEnabled)
        {
            cardModifyingEndpoints.RequireAuthorization();
        }

        cardModifyingEndpoints.MapPost("", 
            (CardCreateModel card, ICardFacade cardFacade) =>
            {
                var id = cardFacade.Create(card);
                return TypedResults.Ok(id);
            });

        cardModifyingEndpoints.MapPut("", Results<Ok<Guid?>, ForbidHttpResult> 
            (CardDetailModel card, ICardFacade cardFacade) =>
            {
                try
                {
                    return TypedResults.Ok(cardFacade.Update(card));
                }
                catch (UnauthorizedAccessException)
                {
                    return TypedResults.Forbid();
                }
            });

        cardModifyingEndpoints.MapPost("upsert",
            (CardDetailModel card, ICardFacade cardFacade) =>
            {
                var id = cardFacade.CreateOrUpdate(card);
                return TypedResults.Ok(id);
            });

        cardModifyingEndpoints.MapDelete("{id:guid}", Results<Ok, ForbidHttpResult> 
            (Guid id, ICardFacade cardFacade) =>
            {
                try
                {
                    cardFacade.Delete(id);
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
