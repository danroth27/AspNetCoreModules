using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Modules
{
    public interface IBeginModuleFeature
    {
        bool PathBaseMatched { get; set; }
        PathString OriginalPathBase { get; set; }

        PathString OriginalPath { get; set; }

        IServiceProvider OriginalRequestServices { get; set; }
    }
}
