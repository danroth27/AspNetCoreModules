using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.PlatformAbstractions;
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
            var modulesPath = Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "Modules");
            var moduleDirs = Directory.EnumerateDirectories(modulesPath);

            foreach (var moduleDir in moduleDirs)
            {   
                if (HasDynamicModule(moduleDir))
                {
                    var moduleDirInfo = new DirectoryInfo(moduleDir);
                    var moduleAssemblyLoadContenxt = new ModuleAssemblyLoadContext(moduleDir);
                    var moduleAssemblyName = new AssemblyName(moduleDirInfo.Name);
                    var moduleAssembly = moduleAssemblyLoadContenxt.LoadFromAssemblyName(moduleAssemblyName);

                    var moduleStartupType = ModuleStartupLoader.FindStartupType(moduleAssembly, env.EnvironmentName);

                    yield return new ModuleDescriptor(moduleStartupType, context.HostingServices, env, context.ModuleOptions);
                }
            }
        }

        bool HasDynamicModule(string moduleDir)
        {
            // TODO: Add real dynamic module detection logic here
            var moduleDirInfo = new DirectoryInfo(moduleDir);
            return File.Exists(Path.Combine(moduleDir, moduleDirInfo.Name + ".dll"));
        }
    }
}
