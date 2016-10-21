using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Modules.Abstractions;
using Microsoft.AspNetCore.Hosting;

namespace Microsoft.AspNetCore.Modules.Mvc
{
    public static class ModulesRouteBuilderExtensions
    {
        public const string ModuleRouteBuilder = "ModuleRouteBuilder";
        public static void ShareRoutes(this IRouteBuilder routes)
        {
            var sharedServices = routes.ApplicationBuilder.ApplicationServices.GetService<ISharedServiceProvider>();
            var moduleManager = sharedServices.GetService<IModuleManager>();
            var env = routes.ApplicationBuilder.ApplicationServices.GetService<IHostingEnvironment>();
            var module = moduleManager.GetModule(env.ApplicationName);
            module.Properties[ModuleRouteBuilder] = routes;
        }
    }
}

