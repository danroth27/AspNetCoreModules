using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Modules
{
    public class ModuleManager : IModuleManager
    {
        IDictionary<string, ModuleDescriptor> _modules = new ConcurrentDictionary<string, ModuleDescriptor>();

        public ModuleManager(IEnumerable<IModuleLoader> moduleLoaders, IServiceCollection hostingServices)
        {
            var sharedModuleServices = new ServiceCollection();
            var moduleDescriptors = moduleLoaders.SelectMany(moduleLoader => moduleLoader.GetModuleDescriptors())
                .Select(moduleDescriptor =>
                {
                    var moduleStartup = moduleDescriptor.ModuleServices.GetRequiredService<IModuleStartup>();
                    moduleStartup.ConfigureSharedServices(sharedModuleServices);
                    return moduleDescriptor;
                })
                .Select(moduleDescriptor => moduleDescriptor.AddSharedServices(sharedModuleServices));

            foreach (var moduleDescriptor in moduleDescriptors)
            {
                _modules[moduleDescriptor.Name] = moduleDescriptor;
            }

            SharedServices = sharedModuleServices.BuildServiceProvider();
        }

        public IServiceProvider SharedServices { get; set; }

        public ModuleDescriptor GetModule(string name)
        {
            ModuleDescriptor module;
            _modules.TryGetValue(name, out module);
            return module;
        }

        public IEnumerable<ModuleDescriptor> GetModules()
        {
            return _modules.Values;
        }
    }
}
