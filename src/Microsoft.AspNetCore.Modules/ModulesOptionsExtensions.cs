using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Modules
{
    public static class ModulesOptionsExtensions
    {
        public static void UseConfiguration(this ModulesOptions options, IConfiguration config)
        {
            foreach (var moduleConfig in config.GetSection("Modules").GetChildren())
            {
                ModuleInstanceOptions moduleInstanceOptions;
                if (!options.ModuleInstanceOptions.TryGetValue(moduleConfig.Key, out moduleInstanceOptions))
                { 
                    options.ModuleInstanceOptions[moduleConfig.Key] = new ModuleInstanceOptions();
                }
                options.ModuleInstanceOptions[moduleConfig.Key].PathBase = moduleConfig["PathBase"];
            }
        }
    }
}
