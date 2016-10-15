using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Modules
{
    public static class ModulesExtensions
    {
        public static void AddModules(this IServiceCollection services)
        {
            // Find all the modules
            // - Determine order, path base, content root, assembly name (app name)
            // Call into modules to add application services

            // Hack to make hosting services available to modules
            var copy = new ServiceCollection();
            foreach (var sd in services)
            {
                copy.Add(sd);
            }
            services.AddSingleton<IServiceCollection>(copy);


        }

        public static void UseModules(this IApplicationBuilder app)
        {
            // For each module (order?):
            // - Determine the path base (how to specify?)
            // - Fork a pipeline for the module's path base
            // - Determine the module's content root
            // - Determine the module's app name (assembly name)
            // - 

        }

        public static IApplicationBuilder UseModule<TStartup>(this IApplicationBuilder app) where TStartup : class
        {
            return app.UseModule<TStartup>(PathString.Empty);
        }

        public static IApplicationBuilder UseModule<TStartup>(this IApplicationBuilder app, PathString pathBase) where TStartup : class
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            if (pathBase.HasValue && pathBase.Value.EndsWith("/", StringComparison.Ordinal))
            {
                throw new ArgumentException("The path must not end with a '/'", nameof(pathBase));
            }

            var moduleStartupType = typeof(TStartup);
            var moduleAssemblyName = moduleStartupType.GetTypeInfo().Assembly.GetName().Name;
            var env = app.ApplicationServices.GetService<IHostingEnvironment>();
            var moduleStartupMethods = StartupLoader.LoadMethods(app.ApplicationServices, moduleStartupType, env.EnvironmentName);

            // Create the module environment
            var moduleEnv = new HostingEnvironment()
            {
                EnvironmentName = env.EnvironmentName,
                ApplicationName = moduleAssemblyName,
                // TODO: Figure out content roots for modules
                ContentRootPath = Path.Combine(env.ContentRootPath, "..", moduleAssemblyName)
                // TODO: Support this in hosting
                // moduleEnv.PathBase = "/hello";
            };

            // Setup the module services
            var hostingServices = app.ApplicationServices.GetService<IServiceCollection>();
            var moduleServices = new ServiceCollection();
            foreach (var sd in hostingServices)
            {
                moduleServices.Add(sd);
            }
            moduleServices.AddSingleton(moduleEnv);
            moduleStartupMethods.ConfigureServicesDelegate(moduleServices);

            // Setup the module pipeline
            var moduleBuilder = app.New();
            moduleBuilder.ApplicationServices = moduleServices.BuildServiceProvider();
            moduleStartupMethods.ConfigureDelegate(moduleBuilder);

            return app.UseMiddleware<ModuleMiddleware>(new ModuleOptions
            {
                ModuleBuilder = moduleBuilder,
                PathBase = pathBase
            });
        }

    }
}
