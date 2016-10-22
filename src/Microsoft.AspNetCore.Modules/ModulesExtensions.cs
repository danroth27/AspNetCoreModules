using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Builder.Internal;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Modules.Abstractions;
using Microsoft.Extensions.Configuration;
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
        public static void AddModules(this IServiceCollection services, IConfiguration config, params Type[] moduleStartupTypes)
        {
            // Find all the modules
            // - Determine order, path base, content root, assembly name (app name)
            // Call into modules to add application services

            // Hack to make hosting services available to modules
            IHostingEnvironment env;
            var hostingServices = GetModuleHostingServices(services, out env);

            var moduleManager = new ModuleManager();
            foreach (var moduleStartupType in moduleStartupTypes)
            {
                var moduleEnv = GetModuleHostingEnvironment(moduleStartupType, env);
                var moduleServices = new ServiceCollection();
                moduleServices.Add(hostingServices);
                moduleServices.AddSingleton(moduleEnv);
                moduleServices.AddSingleton<IModuleStartup>(sp =>
                {
                    var hostingEnvironment = sp.GetRequiredService<IHostingEnvironment>();
                    return new ConventionBasedModuleStartup(
                        ModuleStartupLoader.LoadMethods(sp, moduleStartupType, hostingEnvironment.EnvironmentName));
                });
                var moduleHostingServiceProvider = moduleServices.BuildServiceProvider();
                var moduleStartup = moduleHostingServiceProvider.GetRequiredService<IModuleStartup>();
                moduleStartup.ConfigureSharedServices(services);
                var pathBase = config[$"{moduleEnv.ApplicationName}:PathBase"];
                var moduleDescriptor = new ModuleDescriptor()
                {
                    Name = moduleEnv.ApplicationName,
                    HostingEnvironment = moduleEnv,
                    PathBase = new PathString(pathBase)
                };
                moduleDescriptor.Properties[typeof(IServiceCollection)] = moduleServices;
                moduleDescriptor.Properties[typeof(IModuleStartup)] = moduleStartup;
                moduleManager.AddModule(moduleDescriptor);
            }

            services.AddSingleton<IModuleManager>(moduleManager);
        }

        public static void UseModules(this IApplicationBuilder app)
        {
            // TODO: how to add the shared service provider?
            // moduleServices.AddSingleton<ISharedServiceProvider>(new SharedServiceProvider(app.ApplicationServices));

            var moduleManager = app.ApplicationServices.GetService<IModuleManager>();
            var moduleDescriptors = moduleManager.GetModules();
            foreach (var moduleDescriptor in moduleDescriptors)
            {
                moduleDescriptor.ModuleServices = GetModuleServices(app, moduleDescriptor);
                var moduleBuilder = GetModuleBuilder(app, moduleDescriptor);
                app.UseWhenPath(moduleDescriptor.PathBase, branch =>
                {
                    branch.Use(next =>
                    {
                        moduleBuilder.Run(next);
                        var module = moduleBuilder.Build();
                        return context => module(context);
                    });
                });
            }
        }

        public static IHostingEnvironment GetModuleHostingEnvironment(Type moduleStartupType, IHostingEnvironment env)
        {
            var moduleAssemblyName = moduleStartupType.GetTypeInfo().Assembly.GetName().Name;
            var moduleEnv = new HostingEnvironment();
            moduleEnv.Initialize(
                applicationName: moduleAssemblyName,
                // TODO: Figure out content roots for modules - hack for now
                contentRootPath: Path.Combine(new DirectoryInfo(env.ContentRootPath).Parent.FullName, moduleAssemblyName),
                options: new WebHostOptions() { Environment = env.EnvironmentName });
            return moduleEnv;
        }

        static IServiceCollection GetModuleHostingServices(IServiceCollection hostingServices, out IHostingEnvironment env)
        {
            var moduleServices = new ServiceCollection();
            var hostingServiceProvider = hostingServices.BuildServiceProvider();
            env = null;
            foreach (var sd in hostingServices)
            {
                if (sd.ServiceType == typeof(IHostingEnvironment))
                {
                    env = sd.ImplementationInstance as IHostingEnvironment;
                    continue;
                }
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

        public static IServiceProvider GetModuleServices(IApplicationBuilder app, ModuleDescriptor moduleDescriptor)
        {
            var moduleServices = moduleDescriptor.Properties[typeof(IServiceCollection)] as IServiceCollection;
            var moduleStartup = moduleDescriptor.Properties[typeof(IModuleStartup)] as IModuleStartup;
            moduleServices.AddSingleton<ISharedServiceProvider>(new SharedServiceProvider(app.ApplicationServices));
            return moduleStartup.ConfigureServices(moduleServices);
        }

        static IApplicationBuilder GetModuleBuilder(
            IApplicationBuilder app, 
            ModuleDescriptor moduleDescriptor)
        {
            var moduleStartup = moduleDescriptor.Properties[typeof(IModuleStartup)] as IModuleStartup;
            var moduleBuilder = new ApplicationBuilder(moduleDescriptor.ModuleServices, app.ServerFeatures);
            return moduleBuilder.UseRequestServices(moduleStartup.Configure);
        }

    }
}
