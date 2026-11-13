using Flashcards.Common.Options;
using Flashcards.Web.App;
using Flashcards.Web.BL.Extensions;
using Flashcards.Web.BL.Installers;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// bind options
var apiOptions = builder.Configuration.GetSection(nameof(ApiOptions)).Get<ApiOptions>();
if (apiOptions is null)
{
    throw new ArgumentNullException("Failed to load ApiOptions from configuration.");
}
builder.Services.Configure<ApiOptions>(builder.Configuration.GetSection(nameof(ApiOptions)));

var identityOptions = builder.Configuration.GetSection(nameof(IdentityOptions)).Get<IdentityOptions>();
if (identityOptions is null)
{
    throw new ArgumentNullException("Failed to load IdentityOptions from configuration.");
}
builder.Services.Configure<IdentityOptions>(builder.Configuration.GetSection(nameof(IdentityOptions)));

builder.Services.AddInstaller<WebBLInstaller>(identityOptions, apiOptions);

// OIDC auth
builder.Services.AddOidcAuthentication(options =>
{
    builder.Configuration.Bind("IdentityOptions", options.ProviderOptions);

    // ensure roles scope is requested so role claims are available
    if (!options.ProviderOptions.DefaultScopes.Contains("roles"))
    {
        options.ProviderOptions.DefaultScopes.Add("roles");
    }

    // map role claim type used by the identity server
    options.UserOptions.RoleClaim = "role";
});

// keep existing registration for fallback http client
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

await builder.Build().RunAsync();
