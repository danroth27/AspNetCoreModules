using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace Microsoft.AspNetCore.Modules
{
    public interface IModuleManager
    {
        IServiceProvider SharedServices { get; }

        ModuleDescriptor GetModuleDescriptor(string moduleName);

        IEnumerable<ModuleDescriptor> GetModuleDescriptors();

        ModuleInstance GetModuleInstance(string moduleInstanceId);

        IEnumerable<ModuleInstance> GetModuleInstances();

        IEnumerable<ModuleInstance> UseModules(IApplicationBuilder app);

        ModuleInstance UseModule(IApplicationBuilder app, string moduleName, string moduleInstanceId, PathString pathBase);
    }
}