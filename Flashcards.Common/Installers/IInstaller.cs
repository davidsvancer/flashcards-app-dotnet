using Microsoft.Extensions.DependencyInjection;

namespace Flashcards.Common.Installers
{
    public interface IInstaller
    {
        void Install(IServiceCollection serviceCollection);
    }
}
