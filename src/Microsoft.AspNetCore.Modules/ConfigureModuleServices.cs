using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Modules
{
    public class ConfigureModuleServices : IConfigureModuleServices
    {
        Action<IServiceCollection> _action;

        public ConfigureModuleServices(string moduleName, Action<IServiceCollection> action)
        {
            ModuleName = moduleName;
            _action = action;

        }
        public string ModuleName { get; }

        public IServiceCollection ConfigureServices(IServiceCollection services)
        {
            _action(services);
            return services;
        }
    }
}
