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
using Microsoft.Extensions.FileProviders;

namespace Microsoft.AspNetCore.Modules
{
    public class ModuleDescriptor
    {
        IServiceCollection _hostingServices;
        IHostingEnvironment _env;
        IDictionary<string, ModuleOptions> _moduleOptions;

        public ModuleDescriptor(
            Type moduleStartupType,
            IServiceCollection hostingServices,
            IHostingEnvironment env,
            IDictionary<string, ModuleOptions> moduleOptions)
        {
            _hostingServices = hostingServices;
            _env = env;
            _moduleOptions = moduleOptions;

            ModuleStartupType = moduleStartupType;
            ModuleHostingEnvironment = GetModuleHostingEnvironment();
            Name = ModuleHostingEnvironment.ApplicationName;
            SetupModuleServices();
        }

        public string Name { get; }

        public IHostingEnvironment ModuleHostingEnvironment { get; }

        public IServiceCollection ModuleServiceCollection { get; } = new ServiceCollection();

        public IServiceCollection SharedServices { get; } = new ServiceCollection();

        public Type ModuleStartupType { get; }

        public IDictionary<object, object> Properties { get; } = new ConcurrentDictionary<object, object>();

        void SetupModuleServices()
        {
            AddHostingServices();
            // Module services specified via options should be availabe in the module startup constructor
            ModuleOptions moduleOptions;
            if (_moduleOptions.TryGetValue(Name, out moduleOptions))
            {
                foreach (var configureServices in _moduleOptions[Name].ConfigureServices)
                {
                    configureServices(ModuleServiceCollection);
                }
            }
            var moduleStartup = GetModuleStartup();
            moduleStartup.ConfigureServices(ModuleServiceCollection);
            moduleStartup.ConfigureSharedServices(SharedServices);
        }

        void AddHostingServices()
        {
            ModuleServiceCollection.Add(_hostingServices);
            ModuleServiceCollection.AddSingleton(ModuleHostingEnvironment);
        }

        IModuleStartup GetModuleStartup()
        {
            if (typeof(IModuleStartup).GetTypeInfo().IsAssignableFrom(ModuleStartupType))
            {
                ModuleServiceCollection.AddSingleton(typeof(IModuleStartup), ModuleStartupType);
            }
            else
            {
                ModuleServiceCollection.AddSingleton<IModuleStartup>(sp =>
                {
                    var hostingEnvironment = sp.GetRequiredService<IHostingEnvironment>();
                    return new ConventionBasedModuleStartup(
                        ModuleStartupLoader.LoadMethods(sp, ModuleStartupType, hostingEnvironment.EnvironmentName));
                });
            }
            var moduleHostingServiceProvider = ModuleServiceCollection.BuildServiceProvider();
            return moduleHostingServiceProvider.GetRequiredService<IModuleStartup>();
        }

        IHostingEnvironment GetModuleHostingEnvironment()
        {
            var moduleAssemblyName = ModuleStartupType.GetTypeInfo().Assembly.GetName().Name;
            var moduleEnv = new HostingEnvironment();
            var contentRootPath = Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "Modules", moduleAssemblyName);
            if (!Directory.Exists(contentRootPath))
            {
                Directory.CreateDirectory(contentRootPath);
            }
            moduleEnv.Initialize(
                applicationName: moduleAssemblyName,
                contentRootPath: contentRootPath,
                options: new WebHostOptions() { Environment = _env.EnvironmentName });
            return moduleEnv;
        }
    }
}
