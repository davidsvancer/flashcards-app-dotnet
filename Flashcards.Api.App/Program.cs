using Flashcards.Api.App.Endpoints;
using Flashcards.Api.App.Extensions;
using Flashcards.Api.BL.Facades;
using Flashcards.Api.BL.Mappers;
using Flashcards.Api.BL.Services;
using Flashcards.Api.BL.Services.Interfaces;
using Flashcards.Api.DAL.Common.Interfaces;
using Flashcards.Api.DAL.Common.Repositories;
using Flashcards.Api.DAL.Memory;
using Flashcards.Api.DAL.Memory.Repositories;
using Flashcards.Common;
using Flashcards.Common.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<IdentityOptions>(
    builder.Configuration.GetSection(nameof(IdentityOptions)));

builder.Services.AddOpenApi();
builder.Services.AddOpenApiDocument();
builder.Services.AddHttpContextAccessor();

// --- Services & Facades ---
builder.Services.AddScoped<IUserContext, UserContext>();
builder.Services.AddScoped<ICurrentUserProvider, CurrentUserProvider>();
builder.Services.AddScoped<ISecurityService, SecurityService>();

builder.Services.AddSingleton<IStorage, Storage>();

builder.Services.AddSingleton<CardMapper>();
builder.Services.AddTransient<ICardRepository, CardRepository>();
builder.Services.AddScoped<ICardFacade, CardFacade>();

builder.Services.AddSingleton<UserMapper>();
builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserFacade, UserFacade>();

builder.Services.AddSingleton<CardCollectionMapper>();
builder.Services.AddTransient<ICardCollectionRepository, CardCollectionRepository>();
builder.Services.AddScoped<ICardCollectionFacade, CardCollectionFacade>();

builder.Services.AddSingleton<LessonMapper>();
builder.Services.AddTransient<ILessonRepository, LessonRepository>();
builder.Services.AddScoped<ILessonFacade, LessonFacade>();

builder.Services.AddHttpClient<IIdentityService, DuendeIdentityService>((sp, client) =>
{
    var options = sp.GetRequiredService<IOptions<IdentityOptions>>().Value;
    client.BaseAddress = new Uri(options.Authority);
});

ConfigureCors(builder.Services);
ConfigureAuthentication(builder.Services, builder.Configuration);

var app = builder.Build();

UseSecurityFeatures(app);
UseRouting(app);
UseAuthenticationAndAuthorization(app);
UseEndpoints(app);
UseOpenApi(app);

app.Run();

void UseEndpoints(WebApplication app)
{
    var endpointsBase = app.MapGroup("api")
        .WithOpenApi();

    // Just resolve the IOptions here directly
    var identityOptions = app.Services.GetRequiredService<IOptions<IdentityOptions>>();

    endpointsBase.UseCardEndpoints(identityOptions);
    endpointsBase.UseUserEndpoints(identityOptions);
    endpointsBase.UseCardCollectionEndpoints(identityOptions);
    endpointsBase.UseLessonEndpoints(identityOptions);
}

void ConfigureAuthentication(IServiceCollection serviceCollection, IConfiguration configuration)
{
    serviceCollection.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.Authority = configuration["IdentityOptions:Authority"];
            options.TokenValidationParameters.ValidateAudience = false;
        });

    serviceCollection.AddAuthorizationBuilder()
        .AddPolicy("admin", policy =>
            policy.RequireAuthenticatedUser()
                .RequireRole(UserRoles.Admin)
        );
}

void ConfigureCors(IServiceCollection serviceCollection)
{
    serviceCollection.AddCors(options =>
    {
        options.AddDefaultPolicy(o =>
            o.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod());
    });
}

void UseSecurityFeatures(IApplicationBuilder application)
{
    application.UseCors();
    application.UseHttpsRedirection();
}

void UseAuthenticationAndAuthorization(IApplicationBuilder application)
{
    application.UseAuthentication();
    application.UseAuthorization();
}

void UseRouting(IApplicationBuilder application)
{
    application.UseRouting();
}

void UseOpenApi(IApplicationBuilder application)
{
    application.UseOpenApi();
    application.UseSwaggerUi();
}

public partial class Program { }