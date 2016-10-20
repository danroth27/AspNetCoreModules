using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Modules.Mvc
{
    public static class ModulesMvcServiceCollectionExtensions
    {
        public static void AddModulesMvc(this IServiceCollection services)
        {
            services.AddTransient<ModulesHtmlHelpers>();
        }
    }
}
