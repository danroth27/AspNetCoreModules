using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Builder.Internal;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Modules.Abstractions;
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

            services.AddSingleton<IModuleManager>(new ModuleManager());
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


            // Create the module environment
            var moduleOptions = new WebHostOptions() { Environment = env.EnvironmentName };
            // TODO: Module configuration support
            var moduleEnv = new HostingEnvironment();
            moduleEnv.Initialize(
                applicationName: moduleAssemblyName,
                contentRootPath: Path.Combine(new DirectoryInfo(env.ContentRootPath).Parent.FullName, moduleAssemblyName),
                // TODO: Figure out content roots for modules
                options: new WebHostOptions());
            // TODO: Support this in hosting
            // moduleEnv.PathBase = "/hello";

            // Setup the module services
            var moduleServices = new ServiceCollection();
            // - Copy over the hosting services from the app that were saved previously in AddModules()
            var hostingServices = app.ApplicationServices.GetService<IServiceCollection>();
            var hostingServiceProvider = app.ApplicationServices;
            foreach (var sd in hostingServices)
            {
                if (sd.ServiceType == typeof(IHostingEnvironment)) continue;
                if (!sd.ServiceType.GetTypeInfo().IsGenericTypeDefinition)
                {
                    moduleServices.Add(ServiceDescriptor.Describe(
                        sd.ServiceType,
                        sp => hostingServiceProvider.GetService(sd.ServiceType),
                        sd.Lifetime));
                }
                else
                {
                    moduleServices.Add(sd);
                }
            }
            // - Add the module hosting environment to replace the app hosting environment
            moduleServices.AddSingleton<IHostingEnvironment>(moduleEnv);
            // - Add a shared service provider to allow access to app services
            moduleServices.AddSingleton<ISharedServiceProvider>(new SharedServiceProvider(app.ApplicationServices));
            // - Add module specific services
            moduleServices.AddSingleton<IStartup>(sp =>
            {
                var hostingEnvironment = sp.GetRequiredService<IHostingEnvironment>();
                return new ConventionBasedStartup(StartupLoader.LoadMethods(sp, moduleStartupType, hostingEnvironment.EnvironmentName));
            });
            var moduleHostingServiceProvider = moduleServices.BuildServiceProvider();
            var moduleStartup = moduleHostingServiceProvider.GetRequiredService<IStartup>();
            var moduleServiceProvider = moduleStartup.ConfigureServices(moduleServices);

            // Setup the module pipeline
            var moduleBuilder = new ApplicationBuilder(moduleServiceProvider, app.ServerFeatures);
            moduleBuilder.UseMiddleware<BeginModuleMiddleware>(new ModuleOptions
            {
                ModuleBuilder = moduleBuilder,
                PathBase = pathBase
            });
            moduleStartup.Configure(moduleBuilder);
            moduleBuilder.UseMiddleware<EndModuleMiddleware>();

            var moduleManager = app.ApplicationServices.GetService<IModuleManager>();
            moduleManager.AddModule(new ModuleDescriptor()
            {
                Name = moduleAssemblyName,
                PathBase = pathBase,
                HostingEnvironment = moduleEnv,
                ModuleServices = moduleServiceProvider,

            });

            return app.Use(next =>
            {
                moduleBuilder.Run(next);
                var module = moduleBuilder.Build();
                return context => module(context);
            });
        }

    }
}
