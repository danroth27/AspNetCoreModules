using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Modules
{
    public static class ModuleManagerExtensions
    {
        public static ModuleInstance UseModule(this IModuleManager moduleManager, IApplicationBuilder app, string moduleName)
        {
            return moduleManager.UseModule(app, moduleName, moduleName);
        }

        public static ModuleInstance UseModule(this IModuleManager moduleManager, IApplicationBuilder app, string moduleName, string moduleInstanceId)
        {
            return moduleManager.UseModule(app, moduleName, moduleInstanceId, PathString.Empty);
        }
    }
}
