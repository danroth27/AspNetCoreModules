using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Modules.Mvc
{
    public static class MvcSharedRoutesExtensions
    {
        public static IApplicationBuilder UseMvcWithSharedRoutes(this IApplicationBuilder app, Action<IRouteBuilder> configureRoutes)
        {
            return app.UseMvc(routes =>
            {
                configureRoutes(routes);
                routes.ShareRoutes();
            });
        }

        public static IMvcBuilder AddMvcWithSharedRoutes(this IServiceCollection services)
        {
            services.AddSharedRoutes();
            return services.AddMvc();
        }

        public static IMvcBuilder AddMvcForModules(this IServiceCollection services, Action<MvcOptions> configureOptions)
        {
            var builder = services.AddMvcWithSharedRoutes();
            builder.Services.Configure(configureOptions);
            return builder;
        }
    }
}
