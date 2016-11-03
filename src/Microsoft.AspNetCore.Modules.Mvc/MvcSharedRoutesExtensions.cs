using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Modules;
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
        public static IMvcBuilder AddMvcWithSharedRoutes(this IServiceCollection services)
        {
            services.AddSharedRoutes();
            return services.AddMvc();
        }
        public static IApplicationBuilder UseMvcWithSharedRoutes(this IApplicationBuilder app, Action<IRouteBuilder> configureRoutes)
        {
            return app.UseMvc(routes =>
            {
                configureRoutes(routes);
                routes.ShareRoutes(app.ApplicationServices.GetRequiredService<ModuleInstanceIdProvider>().ModuleInstanceId);
            });
        }
    }
}
