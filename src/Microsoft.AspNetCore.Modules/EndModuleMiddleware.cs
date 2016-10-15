using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Modules
{
    public class EndModuleMiddleware
    {
        readonly RequestDelegate _next;

        public EndModuleMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var modulePathBase = context.Request.PathBase;
            var modulePath = context.Request.Path;
            var moduleRequestServices = context.RequestServices;

            var beginModule = context.Features.Get<IBeginModuleFeature>();
            context.Features.Set<IBeginModuleFeature>(null);
            if (beginModule.PathBaseMatched)
            {
                context.Request.PathBase = beginModule.OriginalPathBase;
                context.Request.Path = beginModule.OriginalPath;
            }
            context.RequestServices = beginModule.OriginalRequestServices;

            await _next(context);

            if (beginModule.PathBaseMatched)
            {
                context.Request.PathBase = modulePathBase;
                context.Request.Path = modulePath;
            }
            context.RequestServices = moduleRequestServices;
        }
    }
}
