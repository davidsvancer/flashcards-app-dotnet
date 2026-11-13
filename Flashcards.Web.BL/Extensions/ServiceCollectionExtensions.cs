using Flashcards.Common.Options;
using Flashcards.Web.BL.Installers;
using Microsoft.Extensions.DependencyInjection;

namespace Flashcards.Web.BL.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddInstaller<TInstaller>(
            this IServiceCollection serviceCollection, IdentityOptions identityOptions, ApiOptions apiOptions)
            where TInstaller : WebBLInstaller, new()
        {
            var installer = new TInstaller();
            installer.Install(serviceCollection, identityOptions, apiOptions);
        }
    }
}