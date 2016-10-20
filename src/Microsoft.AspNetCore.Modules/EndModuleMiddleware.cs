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
            var moduleRequestServices = context.RequestServices;

            var beginModule = context.Features.Get<IBeginModuleFeature>();
            context.Features.Set<IBeginModuleFeature>(null);
            context.RequestServices = beginModule.OriginalRequestServices;

            try
            {
                await _next(context);
            }
            finally
            {
                context.RequestServices = moduleRequestServices;
            }
        }
    }
}
