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

namespace Microsoft.AspNetCore.Modules
{
    public class ModuleDescriptor
    {
        IServiceCollection _hostingServices;

        public ModuleDescriptor(string name, Type moduleStartupType, IHostingEnvironment moduleEnv, PathString pathBase, IServiceCollection hostingServices)
        {
            _hostingServices = hostingServices;
            Name = name;
            ModuleStartupType = moduleStartupType;
            HostingEnvironment = moduleEnv;
            PathBase = pathBase;
            Properties = new ConcurrentDictionary<object, object>();
            ModuleServices = GetModuleServices();
        }

        public string Name { get; }

        public IHostingEnvironment HostingEnvironment { get; }

        public IServiceProvider ModuleServices { get; private set; }

        public PathString PathBase { get; }

        public Type ModuleStartupType { get; }

        public IDictionary<object, object> Properties { get; }

        IServiceProvider GetModuleServices()
        {
            var moduleServices = GetInitialModuleServiceCollection();
            var moduleStartup = GetModuleStartup(moduleServices);
            moduleStartup.ConfigureServices(moduleServices);
            return moduleServices.BuildServiceProvider();
        }

        IServiceProvider GetModuleServices(IServiceCollection sharedServices)
        {
            var moduleServices = GetInitialModuleServiceCollection();
            var moduleStartup = GetModuleStartup(moduleServices);
            moduleServices.Add(sharedServices);
            moduleStartup.ConfigureServices(moduleServices);
            return moduleServices.BuildServiceProvider();
        }

        IServiceCollection GetInitialModuleServiceCollection()
        {
            var moduleServices = new ServiceCollection();
            // TODO: This filtering shouldn't be necessary - fix MVC to pick the last service instead of the first
            moduleServices.Add(_hostingServices.Where(sd => sd.ServiceType != typeof(IHostingEnvironment)));
            moduleServices.AddSingleton(HostingEnvironment);
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

        public ModuleDescriptor AddSharedServices(IServiceCollection sharedServices)
        {
            ModuleServices = GetModuleServices(sharedServices);
            return this;
        }
    }
}
