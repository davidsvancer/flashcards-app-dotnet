using Flashcards.Api.App.EndpointTests.Tests;
using Flashcards.Api.BL.Services.Interfaces;
using Flashcards.Api.DAL.Common.Interfaces;
using Flashcards.Api.DAL.Memory;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
namespace Flashcards.Api.App.EndpointTests
{
    public class TestWebAppFactory : WebApplicationFactory<Program>
    {
        protected override IHost CreateHost(IHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {

                // Remove the real storage
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IStorage));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Add a test one
                services.AddSingleton<IStorage, TestStorage>(s => new TestStorage(false));

                services.AddSingleton<IPolicyEvaluator, FakePolicyEvaluator>();

                services.AddScoped<ICurrentUserProvider, FakeCurrentUserProvider>();
            });

            return base.CreateHost(builder);
        }

    }
}
