using Flashcards.IdentityProvider.App.Endpoints;
using Flashcards.IdentityProvider.App.Services;
using Serilog;
using Serilog.Filters;
using System.Globalization;

namespace Flashcards.IdentityProvider.App
{
    internal static class HostingExtensions
    {
        public static WebApplicationBuilder ConfigureLogging(this WebApplicationBuilder builder)
        {
            builder.Host.UseSerilog((ctx, lc) =>
            {
                lc.WriteTo.Logger(consoleLogger =>
                {
                    consoleLogger.WriteTo.Console(
                        outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}",
                        formatProvider: CultureInfo.InvariantCulture);
                    if (builder.Environment.IsDevelopment())
                    {
                        consoleLogger.Filter.ByExcluding(Matching.FromSource("Duende.IdentityServer.Diagnostics.Summary"));
                    }
                });
                if (builder.Environment.IsDevelopment())
                {
                    lc.WriteTo.Logger(fileLogger =>
                    {
                        fileLogger
                            .WriteTo.File("./diagnostics/diagnostic.log", rollingInterval: RollingInterval.Day,
                                fileSizeLimitBytes: 1024 * 1024 * 10, 
                                rollOnFileSizeLimit: true,
                                outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}",
                                formatProvider: CultureInfo.InvariantCulture)
                            .Filter
                            .ByIncludingOnly(Matching.FromSource("Duende.IdentityServer.Diagnostics.Summary"));
                    }).Enrich.FromLogContext().ReadFrom.Configuration(ctx.Configuration);
                }
            });
            return builder;
        }

        public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddRazorPages();
            
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", policy =>
                {
                    // Allow the client origin used in development. Adjust as needed for production.
                    policy.WithOrigins("https://localhost:44355")
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                    policy.WithOrigins("https://localhost:5001")
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

            builder.Services.AddIdentityServer(options =>
                {
                    // https://docs.duendesoftware.com/identityserver/v6/fundamentals/resources/api_scopes#authorization-based-on-scopes
                    options.EmitStaticAudienceClaim = true;
                })
                .AddInMemoryIdentityResources(Config.IdentityResources)
                .AddInMemoryApiResources(Config.ApiResources)
                .AddInMemoryApiScopes(Config.ApiScopes)
                .AddInMemoryClients(Config.Clients)
                .AddResourceOwnerValidator<ResourceOwnerPasswordValidator>()
                .AddProfileService<LocalAppUserProfileService>();

            return builder.Build();
        }

        public static WebApplication ConfigurePipeline(this WebApplication app)
        {
            app.UseSerilogRequestLogging();

            app.UseCors(policy =>
            {
                policy.AllowAnyHeader();
                policy.AllowAnyMethod();
                policy.AllowAnyOrigin();
            });

            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseRouting();

            app.UseIdentityServer();

            app.UseAuthorization();
            app.MapRazorPages().RequireAuthorization();
            app.UseUserEndpoints();

            return app;
        }
    }
}
