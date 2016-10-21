using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Modules.Mvc
{
    public static class ModulesMvcApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseMvcWithModules(this IApplicationBuilder app, Action<IRouteBuilder> configureRoutes)
        {
            return app.UseMvc(routes =>
            {
                configureRoutes(routes);
                routes.ShareModuleRoutes();
            });
        }
    }
}
