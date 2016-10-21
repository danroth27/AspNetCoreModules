using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Microsoft.AspNetCore.Modules.Abstractions
{
    public class ModuleOptions
    {
        public PathString RoutePrefix { get; set; }

        // TODO: Add a configuration property?
    }
}