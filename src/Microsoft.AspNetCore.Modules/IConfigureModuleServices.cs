using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Modules
{
    public interface IConfigureModuleServices
    {
        string ModuleName { get; }

        IServiceCollection ConfigureServices(IServiceCollection services);
    }
}