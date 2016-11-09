using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace Microsoft.AspNetCore.Modules
{
    public class ModuleLoadContext
    {
        public IHostingEnvironment HostingEnvironment { get; set; }

        public IServiceCollection HostingServices { get; set; }

        public IDictionary<string, ModuleOptions> ModuleOptions { get; set; }
    }
}