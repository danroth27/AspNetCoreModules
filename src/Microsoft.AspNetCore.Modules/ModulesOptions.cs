using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Modules
{
    public class ModulesOptions
    {
        public IList<IModuleLoader> ModuleLoaders { get; } = new List<IModuleLoader>() { new ModuleLoader() };

        public IDictionary<string, ModuleOptions> ModuleOptions { get; } = new Dictionary<string, ModuleOptions>();

        public IDictionary<string, ModuleInstanceOptions> ModuleInstanceOptions { get; } = new Dictionary<string, ModuleInstanceOptions>();

    }
}
