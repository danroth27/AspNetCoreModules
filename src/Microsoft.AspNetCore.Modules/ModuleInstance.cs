using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Modules
{
    public class ModuleInstance
    {
        public ModuleInstance(ModuleDescriptor moduleDescriptor, string moduleInstanceId, PathString pathBase)
        {
            moduleDescriptor.ModuleServiceCollection.AddSingleton<ModuleInstanceIdProvider>(new ModuleInstanceIdProvider(moduleInstanceId));

            ModuleDescriptor = moduleDescriptor;
            ModuleInstanceId = moduleInstanceId;
            ModuleServices = moduleDescriptor.ModuleServiceCollection.BuildServiceProvider();
            PathBase = pathBase;
            Properties = new ConcurrentDictionary<object, object>();
        }

        public string ModuleInstanceId { get; }

        public IServiceProvider ModuleServices { get; }

        public PathString PathBase { get; }

        public ModuleDescriptor ModuleDescriptor { get; }

        public IDictionary<object, object> Properties { get; }


    }
}
