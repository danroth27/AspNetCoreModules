using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.PlatformAbstractions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Modules
{
    public class ModuleLoader : IModuleLoader
    {
        public IEnumerable<ModuleDescriptor> GetModuleDescriptors(ModuleLoadContext context)
        {
            var env = context.HostingEnvironment;
            return ModuleLoaderHelpers.GetModuleStartupTypes(env.ApplicationName, env.EnvironmentName)
                .Select(moduleStartupType => new ModuleDescriptor(moduleStartupType, context.HostingServices, env, context.ModuleOptions));
        }
    }
}
