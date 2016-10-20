using Microsoft.AspNetCore.Modules.Abstractions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Modules
{
    public class ModuleManager : IModuleManager
    {
        IDictionary<string, ModuleDescriptor> _modules = new ConcurrentDictionary<string, ModuleDescriptor>();

        public void AddModule(ModuleDescriptor module)
        {
            _modules[module.Name] = module;
        }

        public ModuleDescriptor GetModule(string name)
        {
            return _modules[name];
        }

        public IEnumerable<ModuleDescriptor> GetModules()
        {
            return _modules.Values;
        }
    }
}
