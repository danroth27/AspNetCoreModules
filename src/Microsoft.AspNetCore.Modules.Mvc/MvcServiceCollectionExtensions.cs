using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Modules.Internal;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace Microsoft.AspNetCore.Modules.Mvc
{
    public static class MvcServiceCollectionExtensions
    {
        public static IServiceCollection AddViewOverrides(this IServiceCollection services)
        {
            var env = services.GetServiceFromCollection<IHostingEnvironment>();
            if (env == null)
            {
                throw new InvalidOperationException("Cannot access hosting environment for module");
            }

            var rootEnv = services.GetServiceFromCollection<IRootHostingEnvironmentAccessor>()?.RootHostingEnvironment;
            if (rootEnv == null)
            {
                throw new InvalidOperationException("Cannot access root hosting environment from module");
            }

            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.FileProviders.Insert(0, new PhysicalFileProvider(Path.Combine(rootEnv.ContentRootPath, "Modules", env.ApplicationName)));
            });
            return services;
        }
    }
}
