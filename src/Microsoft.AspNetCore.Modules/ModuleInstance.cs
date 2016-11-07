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
    public class ModuleInstance
    {
        public ModuleInstance(ModuleDescriptor moduleDescriptor, string moduleInstanceId, PathString pathBase, IEnumerable<IConfigureModuleInstanceServices> configureModuleServices)
        {
            ModuleDescriptor = moduleDescriptor;
            ModuleInstanceId = moduleInstanceId;
            PathBase = pathBase;
            Properties = new ConcurrentDictionary<object, object>();

            IServiceCollection moduleInstanceServices = new ServiceCollection();
            moduleInstanceServices.Add(moduleDescriptor.ModuleServiceCollection);
            moduleInstanceServices.AddSingleton<ModuleInstanceIdProvider>(new ModuleInstanceIdProvider(moduleInstanceId));
            foreach (var config in configureModuleServices)
            {
                config.ConfigureServices(moduleInstanceServices);
            }
            ModuleServices = moduleInstanceServices.BuildServiceProvider();
        }

        public string ModuleInstanceId { get; }

        public IServiceProvider ModuleServices { get; }

        public PathString PathBase { get; }

        public ModuleDescriptor ModuleDescriptor { get; }

        public IDictionary<object, object> Properties { get; }


    }
}
