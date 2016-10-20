using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Modules.Abstractions
{
    public interface IModuleManager
    {
        IEnumerable<ModuleDescriptor> GetModules();

        ModuleDescriptor GetModule(string name);

        void AddModule(ModuleDescriptor module);
    }
}
