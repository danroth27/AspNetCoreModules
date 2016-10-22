using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Modules
{
    public class ModuleStartupMethods : StartupMethods
    {
        public ModuleStartupMethods(
            Action<IApplicationBuilder> configure, 
            Func<IServiceCollection, IServiceProvider> configureServices,
            Func<IServiceCollection, IServiceProvider> configureSharedServices)
            : base(configure, configureServices)
        {
            ConfigureSharedServicesDelegate = configureSharedServices;
        }

        public Func<IServiceCollection, IServiceProvider> ConfigureSharedServicesDelegate { get; }

    }
}
