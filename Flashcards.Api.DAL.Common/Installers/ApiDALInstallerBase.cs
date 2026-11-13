using Flashcards.Common.Installers;
using Microsoft.Extensions.DependencyInjection;

namespace Flashcards.Api.DAL.Common.Installers
{
    public abstract class ApiDALInstallerBase : IInstaller
    {
        public virtual void Install(IServiceCollection sercviceCollection)
        {
        }
    }
}