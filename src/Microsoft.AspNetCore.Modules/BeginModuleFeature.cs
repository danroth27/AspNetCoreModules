using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Microsoft.AspNetCore.Modules
{
    public class BeginModuleFeature : IBeginModuleFeature
    {
        public IServiceProvider OriginalRequestServices { get; set; }
    }
}
