using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Modules
{
    public static class ModuleDescriptorExtensions
    {
        public static ModuleInstance CreateModuleInstance(this ModuleDescriptor moduleDescriptor)
        {
            return moduleDescriptor.CreateModuleInstance(moduleDescriptor.Name);
        }

        public static ModuleInstance CreateModuleInstance(this ModuleDescriptor moduleDescriptor, string moduleInstanceId)
        {
            return moduleDescriptor.CreateModuleInstance(moduleInstanceId, PathString.Empty);
        }

        public static ModuleInstance CreateModuleInstance(this ModuleDescriptor moduleDescriptor, string moduleInstanceId, PathString pathBase)
        {
            return new ModuleInstance(moduleDescriptor, moduleInstanceId, pathBase);
        }
    }
}
