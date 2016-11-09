using Microsoft.Extensions.DependencyModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Modules
{
    public static class ModuleLoaderHelpers
    {
        public static IEnumerable<Type> GetModuleStartupTypes(string entryAssemblyName, string environmentName)
        {
            var entryAssembly = Assembly.Load(new AssemblyName(entryAssemblyName));
            var context = DependencyContext.Load(entryAssembly);
            return context.RuntimeLibraries
                .Where(lib => lib.Name != entryAssemblyName)
                .SelectMany(lib => lib.GetDefaultAssemblyNames(context))
                .Select(assemblyName => ModuleStartupLoader.FindStartupType(assemblyName.Name, environmentName))
                .Where(type => type != null)
                .ToList();
        }
    }
}
