using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Modules
{
    public interface IModuleLoader
    {
        IEnumerable<ModuleDescriptor> GetModuleDescriptors(ModuleLoadContext context);
    }
}
