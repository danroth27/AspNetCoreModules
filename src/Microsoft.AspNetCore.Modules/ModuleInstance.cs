using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Modules
{
    public class ModuleInstance
    {
        public ModuleInstance(
            ModuleDescriptor moduleDescriptor, 
            string moduleInstanceId, 
            PathString pathBase, 
            IServiceCollection sharedServices,
            IServiceProvider appServiceProvider,
            ModuleInstanceOptions options)
        {
            ModuleDescriptor = moduleDescriptor;
            ModuleInstanceId = moduleInstanceId;
            PathBase = pathBase;

            AddSharedServices(sharedServices, appServiceProvider);
            ModuleServiceCollection.Add(moduleDescriptor.ModuleServiceCollection);
            ModuleServiceCollection.AddSingleton<ModuleInstanceIdProvider>(new ModuleInstanceIdProvider(moduleInstanceId));
            ModuleServiceCollection.AddSingleton<IRootServiceProvider>(new RootServiceProvider(appServiceProvider));
            if (options != null)
            {
                foreach (var configureServices in options.ConfigureServices)
                {
                    configureServices(ModuleServiceCollection);
                }
            }
            ModuleServices = ModuleServiceCollection.BuildServiceProvider();
        }

        public string ModuleInstanceId { get; }

        IServiceCollection ModuleServiceCollection { get; } = new ServiceCollection();

        public IServiceProvider ModuleServices { get; }

        public PathString PathBase { get; }

        public ModuleDescriptor ModuleDescriptor { get; }

        public IDictionary<object, object> Properties { get; } = new ConcurrentDictionary<object, object>();

        void AddSharedServices(IServiceCollection sharedServices, IServiceProvider appServiceProvider)
        {
            foreach (var sd in sharedServices)
            {
                if (!sd.ServiceType.GetTypeInfo().IsGenericTypeDefinition && sd.Lifetime != ServiceLifetime.Transient)
                {
                    ModuleServiceCollection.Add(ServiceDescriptor.Describe(
                        sd.ServiceType,
                        sp => appServiceProvider.GetRequiredService(sd.ServiceType),
                        sd.Lifetime));
                }
                else
                {
                    // TODO: How to preserve lifetime for generic services?
                    ModuleServiceCollection.Add(sd);
                }
            }
        }
    }
}
