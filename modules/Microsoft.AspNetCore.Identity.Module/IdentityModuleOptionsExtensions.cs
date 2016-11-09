using Microsoft.AspNetCore.Modules;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Identity.Module
{
    public static class IdentityModuleOptionsExtensions
    {
        public static ModulesOptions ConfigureIdentityModule(this ModulesOptions modulesOptions, IConfiguration config)
        {
            var moduleName = typeof(IdentityModuleOptionsExtensions).GetTypeInfo().Assembly.GetName().Name;
            ModuleOptions moduleOptions = null;
            if (!modulesOptions.ModuleOptions.TryGetValue(moduleName, out moduleOptions))
            {
                modulesOptions.ModuleOptions[moduleName] = new ModuleOptions();
            }
            modulesOptions.ModuleOptions[moduleName].ConfigureServices.Add(services =>
            {
                services.Configure<IdentityModuleOptions>(identityModuleOptions =>
                {
                    identityModuleOptions.ConnectionString = config.GetConnectionString("DefaultConnection");
                });
            });
            return modulesOptions;
        }
    }
}
