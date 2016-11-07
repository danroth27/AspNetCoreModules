using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.Collections.Concurrent;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Reflection;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.AspNetCore.Hosting.Internal;
using System.IO;

namespace Microsoft.AspNetCore.Modules
{
    public class ModuleDescriptor
    {
        IServiceCollection _hostingServices;
        IHostingEnvironment _env;
        IEnumerable<IConfigureModuleServices> _configureModuleServices;

        public ModuleDescriptor(
            Type moduleStartupType,
            IServiceCollection hostingServices,
            IHostingEnvironment env,
            IEnumerable<IConfigureModuleServices> configureModuleServices)
        {
            _hostingServices = hostingServices;
            _env = env;
            _configureModuleServices = configureModuleServices;

            ModuleStartupType = moduleStartupType;
            HostingEnvironment = GetModuleHostingEnvironment();
            Name = HostingEnvironment.ApplicationName;
            ModuleServiceCollection = GetModuleServices();
            Properties = new ConcurrentDictionary<object, object>();
        }

        public string Name { get; }

        public IHostingEnvironment HostingEnvironment { get; }

        public IServiceCollection ModuleServiceCollection { get; }

        public Type ModuleStartupType { get; }

        public IDictionary<object, object> Properties { get; }

        IServiceCollection GetModuleServices()
        {
            var moduleServices = new ServiceCollection();
            // TODO: This filtering shouldn't be necessary - fix MVC to pick the last service instead of the first
            moduleServices.Add(_hostingServices.Where(sd => sd.ServiceType != typeof(IHostingEnvironment)));
            moduleServices.AddSingleton(HostingEnvironment);
            foreach (var config in _configureModuleServices.Where(config => config.ModuleName == Name))
            {
                config.ConfigureServices(moduleServices);
            }
            var moduleStartup = GetModuleStartup(moduleServices);
            moduleStartup.ConfigureServices(moduleServices);
            return moduleServices;
        }

        IModuleStartup GetModuleStartup(IServiceCollection moduleServices)
        {
            if (typeof(IModuleStartup).GetTypeInfo().IsAssignableFrom(ModuleStartupType))
            {
                moduleServices.AddSingleton(typeof(IModuleStartup), ModuleStartupType);
            }
            else
            {
                moduleServices.AddSingleton<IModuleStartup>(sp =>
                {
                    var hostingEnvironment = sp.GetRequiredService<IHostingEnvironment>();
                    return new ConventionBasedModuleStartup(
                        ModuleStartupLoader.LoadMethods(sp, ModuleStartupType, hostingEnvironment.EnvironmentName));
                });
            }
            var moduleHostingServiceProvider = moduleServices.BuildServiceProvider();
            return moduleHostingServiceProvider.GetRequiredService<IModuleStartup>();
        }

        IHostingEnvironment GetModuleHostingEnvironment()
        {
            var moduleAssemblyName = ModuleStartupType.GetTypeInfo().Assembly.GetName().Name;
            var moduleEnv = new HostingEnvironment();
            moduleEnv.Initialize(
                applicationName: moduleAssemblyName,
                contentRootPath: Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "Modules", moduleAssemblyName),
                options: new WebHostOptions() { Environment = _env.EnvironmentName });
            return moduleEnv;
        }
    }
}
