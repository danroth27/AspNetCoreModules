using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Builder.Internal;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Modules.Internal;

namespace Microsoft.AspNetCore.Modules
{
    public class ModuleManager : IModuleManager
    {
        IDictionary<string, ModuleDescriptor> _modulesDescriptors = new ConcurrentDictionary<string, ModuleDescriptor>();
        IDictionary<string, ModuleInstance> _moduleInstances = new ConcurrentDictionary<string, ModuleInstance>();
        ModulesOptions _options;

        IServiceCollection _hostingServices;

        public ModuleManager(IServiceCollection services, ModulesOptions options)
        {
            _options = options;

            _hostingServices = CreateHostingServices(services);
            var moduleLoadContext = new ModuleLoadContext()
            {
                HostingEnvironment = services.GetServiceFromCollection<IHostingEnvironment>(),
                HostingServices = _hostingServices,
                ModuleOptions = options.ModuleOptions
            };
            var moduleDescriptors = _options.ModuleLoaders
                .SelectMany(moduleLoader => moduleLoader.GetModuleDescriptors(moduleLoadContext));

            foreach (var moduleDescriptor in moduleDescriptors)
            {
                var moduleStartup = moduleDescriptor.ModuleServiceCollection.BuildServiceProvider().GetRequiredService<IModuleStartup>();
                moduleStartup.ConfigureSharedServices(SharedServices);
                _modulesDescriptors[moduleDescriptor.Name] = moduleDescriptor;
            }
        }

        public IServiceCollection SharedServices { get; } = new ServiceCollection();

        static IServiceCollection CreateHostingServices(IServiceCollection services)
        {
            var hostingServices = new ServiceCollection();

            hostingServices.AddSingleton(services.GetServiceFromCollection<ILoggerFactory>());
            hostingServices.AddLogging();
            hostingServices.AddSingleton(services.GetServiceFromCollection<DiagnosticListener>());
            hostingServices.AddSingleton(services.GetServiceFromCollection<DiagnosticSource>());
            hostingServices.AddOptions();

            // This is used as part of supporting ConfigureContainer on startup
            hostingServices.AddTransient<IServiceProviderFactory<IServiceCollection>, DefaultServiceProviderFactory>();

            // The object pool isn't preserved today from the hosting container - possible bug?
            hostingServices.AddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();

            // TODO: Need a way to pass along the IServer
            // hostingServices.AddSingleton<IServer>(server)

            var env = services.GetServiceFromCollection<IHostingEnvironment>();
            if (env != null)
            {
                hostingServices.AddSingleton<IRootHostingEnvironmentAccessor>(new RootHostingEnvironmentAccessor(env));
            }

            return hostingServices;
        }

        public ModuleDescriptor GetModuleDescriptor(string name)
        {
            ModuleDescriptor module;
            _modulesDescriptors.TryGetValue(name, out module);
            return module;
        }

        public IEnumerable<ModuleDescriptor> GetModuleDescriptors()
        {
            return _modulesDescriptors.Values;
        }

        public IEnumerable<ModuleInstance> GetModuleInstances()
        {
            return _moduleInstances.Values;
        }

        public ModuleInstance GetModuleInstance(string moduleInstanceId)
        {
            ModuleInstance moduleInstance;
            _moduleInstances.TryGetValue(moduleInstanceId, out moduleInstance);
            return moduleInstance;
        }
        
        public IEnumerable<ModuleInstance> UseModules(IApplicationBuilder app)
        {
            foreach (var moduleDescriptor in GetModuleDescriptors())
            {
                var moduleName = moduleDescriptor.Name;
                ModuleInstanceOptions moduleInstanceOptions;
                _options.ModuleInstanceOptions.TryGetValue(moduleName, out moduleInstanceOptions);
                UseModule(app, moduleName, moduleName, moduleInstanceOptions?.PathBase);
            }
            return _moduleInstances.Values;
        }

        public ModuleInstance UseModule(IApplicationBuilder app, string moduleName, string moduleInstanceId, PathString pathBase)
        {
            if (_moduleInstances.ContainsKey(moduleInstanceId))
            {
                throw new InvalidOperationException($"A module with instance ID {moduleInstanceId} is already in use");
            }

            if (!_modulesDescriptors.ContainsKey(moduleName))
            {
                throw new InvalidOperationException($"Module {moduleName} is not loaded");
            }

            var moduleDescriptor = GetModuleDescriptor(moduleName);
            ModuleInstanceOptions moduleInstanceOptions;
            _options.ModuleInstanceOptions.TryGetValue(moduleInstanceId, out moduleInstanceOptions);
            var moduleInstance = new ModuleInstance(
                moduleDescriptor,
                moduleInstanceId,
                pathBase,
                SharedServices,
                app.ApplicationServices,
                moduleInstanceOptions);
            _moduleInstances.Add(moduleInstance.ModuleInstanceId, moduleInstance);

            var moduleBuilder = GetModuleBuilder(app, moduleInstance);
            app.UseWhenPath(pathBase, branch =>
            {
                branch.Use(next =>
                {
                    moduleBuilder.Run(next);
                    var module = moduleBuilder.Build();
                    return context => module(context);
                });
            });

            return moduleInstance;
        }

        static IApplicationBuilder GetModuleBuilder(
            IApplicationBuilder app,
            ModuleInstance moduleInstance)
        {
            var moduleStartup = moduleInstance.ModuleServices.GetRequiredService<IModuleStartup>();
            var moduleBuilder = new ApplicationBuilder(moduleInstance.ModuleServices, app.ServerFeatures);
            return moduleBuilder.UseRequestServices(moduleStartup.Configure);
        }
    }
}
