using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Options;
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
        ModulesOptions _options;
        IServiceCollection _hostingServices;

        public ModuleLoader(IHostingEnvironment env, IServiceCollection hostingServices, IOptions<ModulesOptions> modulesOptions)
        {
            _env = env;
            _options = modulesOptions.Value;
            _hostingServices = hostingServices;
        }

        public IEnumerable<ModuleDescriptor> GetModuleDescriptors()
        {
            return GetModuleStartupTypes(_env.ApplicationName)
                .Select(moduleStartupType => GetModuleDescriptor(moduleStartupType));
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

        ModuleDescriptor GetModuleDescriptor(Type moduleStartupType)
        {
            var moduleEnv = GetModuleHostingEnvironment(moduleStartupType);
            var moduleName = moduleEnv.ApplicationName;
            var pathBase = PathString.Empty;
            _options.PathBase.TryGetValue(moduleName, out pathBase);

            return new ModuleDescriptor(moduleName, moduleStartupType, moduleEnv, pathBase, _hostingServices);
        }

        IHostingEnvironment GetModuleHostingEnvironment(Type moduleStartupType)
        {
            var moduleAssemblyName = moduleStartupType.GetTypeInfo().Assembly.GetName().Name;
            var moduleEnv = new HostingEnvironment();
            moduleEnv.Initialize(
                applicationName: moduleAssemblyName,
                contentRootPath: Path.Combine(_env.ContentRootPath, "Modules", moduleAssemblyName),
                options: new WebHostOptions() { Environment = _env.EnvironmentName });
            return moduleEnv;
        }
    }
}
