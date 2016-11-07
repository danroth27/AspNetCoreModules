using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Modules
{
    public class ConfigureModuleInstanceServices : IConfigureModuleInstanceServices
    {
        Action<IServiceCollection> _action;

        public ConfigureModuleInstanceServices(string moduleInstanceId, Action<IServiceCollection> action)
        {
            ModuleInstanceId = moduleInstanceId;
            _action = action;

        }
        public string ModuleInstanceId { get; }

        public IServiceCollection ConfigureServices(IServiceCollection services)
        {
            _action(services);
            return services;
        }
    }
}
