using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Builder.Internal;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Modules.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
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
            return app.UseModuleCore<TStartup>(PathString.Empty);
        }

        public static IApplicationBuilder UseModuleAtPath<TStartup>(
            this IApplicationBuilder app,
            PathString pathBase) where TStartup : class
        {
            return app.UseWhenPath(pathBase, branch =>
            {
                branch.UseModuleCore<TStartup>(pathBase);
            });
        }

        public static IApplicationBuilder UseModuleWhen<TStartup>(
            this IApplicationBuilder app,
            Func<HttpContext, bool> predicate) where TStartup : class
        {
            return app.UseWhen(predicate, branch =>
            {
                branch.UseModule<TStartup>();
            });
        }

        public static IApplicationBuilder UseModuleCore<TStartup>(this IApplicationBuilder app, PathString pathBase) where TStartup : class
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            var moduleEnv = GetModuleHostingEnvironment<TStartup>(app);

            IStartup moduleStartup;
            var moduleServices = GetModuleServices<TStartup>(app, moduleEnv, out moduleStartup);

            AddModuleDescriptor(app, moduleEnv.ApplicationName, moduleEnv, moduleServices, pathBase);

            var moduleBuilder = GetModuleBuilder(app, moduleServices, moduleStartup.Configure);

            // Wire the module into the app pipeline
            return app.Use(next =>
            {
                moduleBuilder.Run(next);
                var module = moduleBuilder.Build();
                return context => module(context);
            });
        }

        public static IHostingEnvironment GetModuleHostingEnvironment<TStartup>(IApplicationBuilder app) where TStartup : class
        {
            var moduleStartupType = typeof(TStartup);
            var moduleAssemblyName = moduleStartupType.GetTypeInfo().Assembly.GetName().Name;
            var env = app.ApplicationServices.GetService<IHostingEnvironment>();
            var moduleEnv = new HostingEnvironment();
            moduleEnv.Initialize(
                applicationName: moduleAssemblyName,
                // TODO: Figure out content roots for modules - hack for now
                contentRootPath: Path.Combine(new DirectoryInfo(env.ContentRootPath).Parent.FullName, moduleAssemblyName),
                options: new WebHostOptions() { Environment = env.EnvironmentName });
            return moduleEnv;
        }

        public static IServiceProvider GetModuleServices<TStartup>(IApplicationBuilder app, IHostingEnvironment moduleEnv, out IStartup moduleStartup) where TStartup : class
        {
            var moduleServices = new ServiceCollection();
            moduleServices.Add(GetModuleHostingServices(app));
            moduleServices.AddSingleton<IHostingEnvironment>(moduleEnv);
            moduleServices.AddSingleton<ISharedServiceProvider>(new SharedServiceProvider(app.ApplicationServices));
            moduleServices.AddSingleton<IStartup>(sp =>
            {
                var hostingEnvironment = sp.GetRequiredService<IHostingEnvironment>();
                return new ConventionBasedStartup(StartupLoader.LoadMethods(sp, typeof(TStartup), hostingEnvironment.EnvironmentName));
            });
            var moduleHostingServiceProvider = moduleServices.BuildServiceProvider();
            moduleStartup = moduleHostingServiceProvider.GetRequiredService<IStartup>();
            return moduleStartup.ConfigureServices(moduleServices);
        }

        static IServiceCollection GetModuleHostingServices(IApplicationBuilder app)
        {
            var moduleServices = new ServiceCollection();
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
            return moduleServices;
        }

        static void AddModuleDescriptor(
            IApplicationBuilder app, 
            string name, 
            IHostingEnvironment moduleEnv, 
            IServiceProvider moduleServices,
            PathString pathBase)
        {
            var moduleManager = app.ApplicationServices.GetService<IModuleManager>();
            moduleManager.AddModule(new ModuleDescriptor()
            {
                Name = moduleEnv.ApplicationName,
                HostingEnvironment = moduleEnv,
                ModuleServices = moduleServices,
                PathBase = pathBase
            });
        }

        static IApplicationBuilder GetModuleBuilder(
            IApplicationBuilder app, 
            IServiceProvider moduleServices, 
            Action<IApplicationBuilder> configuration)
        {
            var moduleBuilder = new ApplicationBuilder(moduleServices, app.ServerFeatures);
            return moduleBuilder.UseRequestServices(configuration);
        }

    }
}
