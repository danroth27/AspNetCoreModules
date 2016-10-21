using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Modules
{
    public static class UseWhenPathExtensions
    {
        public static IApplicationBuilder UseWhenPath(
            this IApplicationBuilder app, 
            PathString pathBase, 
            Action<IApplicationBuilder> configuration)
        {
            PathString originalPath;
            PathString originalPathBase;
            return app.UseWhen(
                context =>
                {
                    originalPath = context.Request.Path;
                    originalPathBase = context.Request.PathBase;
                    return context.Request.Path.StartsWithSegments(pathBase);
                },
                subApp =>
                {
                    subApp.UsePathBase(pathBase);
                    configuration(subApp);
                    subApp.Use(async (context, next) =>
                    {
                        var virtualPath = context.Request.Path;
                        context.Request.Path = originalPath;
                        context.Request.PathBase = originalPathBase;
                        try
                        {
                            await next();
                        }
                        finally
                        {
                            context.Request.Path = virtualPath;
                            context.Request.PathBase = pathBase;
                        }
                    });
                });
        }
    }
}
