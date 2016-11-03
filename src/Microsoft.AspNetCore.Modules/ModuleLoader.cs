using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.PlatformAbstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Modules
{
    public class ModuleLoader : IModuleLoader
    {
        IHostingEnvironment _env;
        IServiceCollection _hostingServices;

        public ModuleLoader(IHostingEnvironment env, IServiceCollection hostingServices)
        {
            _env = env;
            _hostingServices = hostingServices;
        }

        public IEnumerable<ModuleDescriptor> GetModuleDescriptors()
        {
            return GetModuleStartupTypes(_env.ApplicationName)
                .Select(moduleStartupType => new ModuleDescriptor(moduleStartupType, _hostingServices, _env));
        }

        IEnumerable<Type> GetModuleStartupTypes(string entryAssemblyName)
        {
            var entryAssembly = Assembly.Load(new AssemblyName(entryAssemblyName));
            var context = DependencyContext.Load(entryAssembly);
            return context.RuntimeLibraries
                .Where(lib => lib.Name != entryAssemblyName)
                .SelectMany(lib => lib.GetDefaultAssemblyNames(context))
                .Select(assemblyName => ModuleStartupLoader.FindStartupType(assemblyName.Name, _env.EnvironmentName))
                .Where(type => type != null)
                .ToList();
        }
    }
}
