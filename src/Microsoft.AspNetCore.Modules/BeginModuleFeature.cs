using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Microsoft.AspNetCore.Modules
{
    public class BeginModuleFeature : IBeginModuleFeature
    {
        public bool PathBaseMatched { get; set; }

        public PathString OriginalPathBase { get; set; }
        public PathString OriginalPath { get; set; }

        public IServiceProvider OriginalRequestServices { get; set; }
    }
}
