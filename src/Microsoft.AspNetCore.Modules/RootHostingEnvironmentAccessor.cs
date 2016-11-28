using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace Microsoft.AspNetCore.Modules
{
    public class RootHostingEnvironmentAccessor : IRootHostingEnvironmentAccessor
    {
        public RootHostingEnvironmentAccessor(IHostingEnvironment rootEnv)
        {
            RootHostingEnvironment = rootEnv;
        }

        public IHostingEnvironment RootHostingEnvironment { get; }
    }
}
