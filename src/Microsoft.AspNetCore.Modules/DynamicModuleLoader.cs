using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Modules
{
    public class DynamicModuleLoader : IModuleLoader
    {
        public IEnumerable<ModuleDescriptor> GetModuleDescriptors(ModuleLoadContext context)
        {
            var env = context.HostingEnvironment;
            var moduleDirs = env.ContentRootFileProvider.GetDirectoryContents("Modules")
                .Where(fileInfo => fileInfo.IsDirectory);

            foreach (var moduleDir in moduleDirs)
            {
                var moduleAssemblyLoadContenxt = new ModuleAssemblyLoadContext(moduleDir.PhysicalPath);
                var moduleAssemblyName = new AssemblyName(moduleDir.Name);
                var moduleAssembly = moduleAssemblyLoadContenxt.LoadFromAssemblyName(moduleAssemblyName);

                var moduleStartupType = ModuleStartupLoader.FindStartupType(moduleAssembly, env.EnvironmentName);

                yield return new ModuleDescriptor(moduleStartupType, context.HostingServices, env, context.ModuleOptions);
            }
        }
    }
}
