using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Modules
{
    public interface IRootHostingEnvironmentAccessor
    {
        IHostingEnvironment RootHostingEnvironment { get; }
    }
}
