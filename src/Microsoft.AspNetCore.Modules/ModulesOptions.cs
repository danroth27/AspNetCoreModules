using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Modules
{
    public class ModulesOptions
    {
        public ModulesOptions()
        {
            PathBase = new Dictionary<string, PathString>();
        }
        public IDictionary<string, PathString> PathBase { get; }
    }
}
