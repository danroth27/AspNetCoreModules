using System.Collections.Generic;
using System.Linq;

namespace Microsoft.AspNetCore.Modules
{
    public class ModuleLoader : IModuleLoader
    {
        public IEnumerable<ModuleDescriptor> GetModuleDescriptors(ModuleLoadContext context)
        {
            var env = context.HostingEnvironment;
            return ModuleStartupLoader.GetModuleStartupTypes(env.ApplicationName, env.EnvironmentName)
                .Select(moduleStartupType => new ModuleDescriptor(moduleStartupType, context.HostingServices, env, context.ModuleOptions));
        }
    }
}
