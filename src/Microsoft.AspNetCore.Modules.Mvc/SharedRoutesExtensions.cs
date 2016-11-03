using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.AspNetCore.Modules.Mvc
{
    public static class SharedRoutesExtensions
    {
        public static void ShareRoutes(this IRouteBuilder routes, string name)
        {
            var moduleRouteManager = routes.ApplicationBuilder.ApplicationServices.GetRequiredService<ISharedRoutesManager>();
            moduleRouteManager.ShareRoutes(name, routes);
        }

        public static void AddSharedRoutes(this IServiceCollection services)
        {
            services.TryAdd(ServiceDescriptor.Singleton<ISharedRoutesManager, SharedRoutesManager>());
        }
    }
}

