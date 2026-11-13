using Microsoft.Extensions.DependencyInjection;
using Flashcards.Common.Installers;

namespace Flashcards.Common.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddInstaller<TInstaller>(this IServiceCollection serviceCollection)
            where TInstaller : IInstaller, new()
        {
            var installer = new TInstaller();
            installer.Install(serviceCollection);
        }
    }
}