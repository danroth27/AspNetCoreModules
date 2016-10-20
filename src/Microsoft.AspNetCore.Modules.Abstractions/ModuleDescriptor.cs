using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Microsoft.AspNetCore.Modules.Abstractions
{
    public class ModuleDescriptor
    {
        public string Name { get; set; }

        public PathString PathBase { get; set; }

        public IHostingEnvironment HostingEnvironment { get; set; }

        public IServiceProvider ModuleServices { get; set; }

    }
}
