using Flashcards.Api.App.Endpoints;
using Flashcards.Common.Options;
using Microsoft.Extensions.Options;

namespace Flashcards.Api.App.Extensions;

public static class EndpointRouteBuilderExtensions
{
    public static IEndpointRouteBuilder UseCardCollectionEndpoints(
        this IEndpointRouteBuilder endpointRouteBuilder,
        IOptions<IdentityOptions> identityOptions)
        => new CardCollectionEndpoints(identityOptions)
        .MapEndpoints(endpointRouteBuilder);

    public static IEndpointRouteBuilder UseUserEndpoints(
        this IEndpointRouteBuilder endpointRouteBuilder,
        IOptions<IdentityOptions> identityOptions)
        => new UserEndpoints(identityOptions)
        .MapEndpoints(endpointRouteBuilder);

    public static IEndpointRouteBuilder UseCardEndpoints(
        this IEndpointRouteBuilder endpointRouteBuilder,
        IOptions<IdentityOptions> identityOptions)
        => new CardEndpoints(identityOptions)
            .MapEndpoints(endpointRouteBuilder);

    public static IEndpointRouteBuilder UseLessonEndpoints(
        this IEndpointRouteBuilder endpointRouteBuilder,
        IOptions<IdentityOptions> identityOptions)
        => new LessonEndpoints(identityOptions)
            .MapEndpoints(endpointRouteBuilder);
}