using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Modules.Abstractions;

namespace Microsoft.AspNetCore.Modules
{
    public static class RoutePrefixApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseModuleRoutePrefix(this IApplicationBuilder app, Action<IApplicationBuilder> configuration)
        {
            var routePrefix = app.ApplicationServices.GetService<IOptions<ModuleOptions>>().Value.RoutePrefix;

            PathString originalPath;
            PathString originalPathBase;
            return app.UseWhen(
                context =>
                {
                    originalPath = context.Request.Path;
                    originalPathBase = context.Request.PathBase;
                    return context.Request.Path.StartsWithSegments(routePrefix);
                },
                subApp =>
                {
                    subApp.UsePathBase(routePrefix);
                    configuration(subApp);
                    subApp.Use((context, next) =>
                    {
                        context.Request.Path = originalPath;
                        context.Request.PathBase = originalPathBase;
                        return next();
                    });
                });
        }
    }
}
