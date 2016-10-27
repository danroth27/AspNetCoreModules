using System;
using System.Collections.Generic;

namespace Microsoft.AspNetCore.Modules
{
    public interface IModuleManager
    {
        IServiceProvider SharedServices { get; set; }

        ModuleDescriptor GetModule(string name);

        IEnumerable<ModuleDescriptor> GetModules();
    }
}