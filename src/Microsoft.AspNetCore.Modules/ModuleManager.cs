using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Builder.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Modules
{
    public class ModuleManager : IModuleManager
    {
        IDictionary<string, ModuleDescriptor> _modulesDescriptors = new ConcurrentDictionary<string, ModuleDescriptor>();
        IDictionary<string, ModuleInstance> _moduleInstances = new ConcurrentDictionary<string, ModuleInstance>();
        ModulesOptions _options;
        IDictionary<string, IEnumerable<IConfigureModuleInstanceServices>> _configureModuleInstanceServices;

        public ModuleManager(
            IEnumerable<IModuleLoader> moduleLoaders,
            IServiceCollection hostingServices,
            IOptions<ModulesOptions> options,
            IEnumerable<IConfigureModuleInstanceServices> configureModuleInstanceServices)
        {
            _options = options.Value;
            _configureModuleInstanceServices = new ConcurrentDictionary<string, IEnumerable<IConfigureModuleInstanceServices>>(configureModuleInstanceServices
                .GroupBy(config => config.ModuleInstanceId)
                .ToDictionary(group => group.Key, group => group.AsEnumerable()));

            var sharedModuleServices = new ServiceCollection();
            var moduleDescriptors = moduleLoaders.SelectMany(moduleLoader => moduleLoader.GetModuleDescriptors());

            foreach (var moduleDescriptor in moduleDescriptors)
            {
                var moduleStartup = moduleDescriptor.ModuleServiceCollection.BuildServiceProvider().GetRequiredService<IModuleStartup>();
                moduleStartup.ConfigureSharedServices(sharedModuleServices);
                _modulesDescriptors[moduleDescriptor.Name] = moduleDescriptor;
            }

            SharedServices = sharedModuleServices.BuildServiceProvider();
        }

        public IServiceProvider SharedServices { get; }

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
                PathString pathBase;
                _options.PathBase.TryGetValue(moduleName, out pathBase);
                UseModule(app, moduleName, moduleName, pathBase);
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
            var moduleInstanceServices = GetModuleInstanceServices(moduleInstanceId);
            var moduleInstance = new ModuleInstance(moduleDescriptor, moduleInstanceId, pathBase, moduleInstanceServices);
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

        IEnumerable<IConfigureModuleInstanceServices> GetModuleInstanceServices(string moduleInstanceId)
        {
            IEnumerable<IConfigureModuleInstanceServices> configureModuleServices;
            if (_configureModuleInstanceServices.TryGetValue(moduleInstanceId, out configureModuleServices))
            {
                return configureModuleServices;
            }
            else
            {
                return new IConfigureModuleInstanceServices[0];
            }
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
