using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Microsoft.AspNetCore.Modules
{
    public class ModuleOptions
    {
        /// <summary>
        /// The branch taken for a positive match.
        /// </summary>
        public IApplicationBuilder ModuleBuilder { get; set; }

        public PathString PathBase { get; set; }
    }
}