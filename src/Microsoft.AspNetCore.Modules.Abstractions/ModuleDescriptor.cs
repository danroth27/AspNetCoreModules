using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.Collections.Concurrent;
using Microsoft.AspNetCore.Builder;

namespace Microsoft.AspNetCore.Modules.Abstractions
{
    public class ModuleDescriptor
    {
        public ModuleDescriptor()
        {
            Properties = new ConcurrentDictionary<string, object>();
        }

        public string Name { get; set; }

        public IHostingEnvironment HostingEnvironment { get; set; }

        public IServiceProvider ModuleServices { get; set; }

        public PathString PathBase { get; set; }

        public IDictionary<string, object> Properties { get; }
    }
}
