using Microsoft.AspNetCore.Http;

namespace Microsoft.AspNetCore.Modules
{
    public class ModuleOptions
    {
        /// <summary>
        /// The branch taken for a positive match.
        /// </summary>
        public RequestDelegate Module { get; set; }
    }
}