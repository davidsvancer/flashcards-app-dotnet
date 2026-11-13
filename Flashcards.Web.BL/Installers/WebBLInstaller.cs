using Flashcards.Common.Options;
using Flashcards.Web.BL.Api;
using Flashcards.Web.BL.Facades;
using Flashcards.Web.BL.MessageHandlers;
using Microsoft.Extensions.DependencyInjection;

namespace Flashcards.Web.BL.Installers;

public class WebBLInstaller
{
    public void Install(
        IServiceCollection serviceCollection,
        IdentityOptions identityOptions,
        ApiOptions apiOptions)
    {
        // Message handler provides token or fallback behaviour
        serviceCollection.AddScoped<CustomAuthorizationMessageHandler>();

        AddApiClient<ICardCollectionApiClient, CardCollectionApiClient>(serviceCollection, identityOptions, apiOptions);
        AddApiClient<IUserApiClient, UserApiClient>(serviceCollection, identityOptions, apiOptions);
        AddApiClient<ILessonApiClient, LessonApiClient>(serviceCollection, identityOptions, apiOptions);
        AddApiClient<ICardApiClient, CardApiClient>(serviceCollection, identityOptions, apiOptions);

        serviceCollection.AddTransient<CardCollectionFacade>();
        serviceCollection.AddTransient<LessonFacade>();
        serviceCollection.AddTransient<CardFacade>();
        serviceCollection.AddTransient<UserFacade>();
    }

    private void AddApiClient<TInterface, TImplementation>(
        IServiceCollection serviceCollection,
        IdentityOptions identityOptions,
        ApiOptions apiOptions)
        where TInterface : class, IApiClient
        where TImplementation : class, TInterface
    {
        var httpClient = serviceCollection.AddHttpClient<TInterface, TImplementation>(client =>
        {
            client.BaseAddress = new Uri(apiOptions.BaseUrl);
        });

        if (identityOptions.IsIdentityEnabled)
        {
            httpClient.AddHttpMessageHandler(serviceProvider
                => serviceProvider.GetRequiredService<CustomAuthorizationMessageHandler>()
                    .ConfigureHandler(
                        authorizedUrls: [apiOptions.BaseUrl],
                        scopes: identityOptions.DefaultScopes));
        }
    }
}
