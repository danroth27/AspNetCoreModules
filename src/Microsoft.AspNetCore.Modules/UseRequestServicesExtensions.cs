using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Modules
{
    public static class UseRequestServicesExtensions
    {
        public static IApplicationBuilder UseRequestServices(
            this IApplicationBuilder app, 
            Action<IApplicationBuilder> configuration)
        {
            var scopeFactory = app.ApplicationServices.GetService<IServiceScopeFactory>();
            IServiceProvider requestServices = null;
            IServiceProvider originalRequestServices = null;
            app.Use(async (context, next) =>
            {
                originalRequestServices = context.RequestServices;
                using (var scope = scopeFactory.CreateScope())
                {
                    requestServices = GetRequestServices(scope, context);
                    context.RequestServices = requestServices;
                    try
                    {
                        await next();
                    }
                    finally
                    {
                        context.RequestServices = originalRequestServices;
                    }
                }
            });
            configuration(app);
            return app.Use(async (context, next) =>
            {
                context.RequestServices = originalRequestServices;
                try
                {
                    await next();
                }
                finally
                {
                    context.RequestServices = requestServices;
                }
            });
        }

        // Need to fixup the IHttpContextAccessor
        static IServiceProvider GetRequestServices(IServiceScope scope, HttpContext context)
        {
            var requestServices = scope.ServiceProvider;
            var httpContextAccessor = requestServices.GetService<IHttpContextAccessor>();
            if (httpContextAccessor != null)
            {
                httpContextAccessor.HttpContext = context;
            }
            return requestServices;
        }
    }
}
