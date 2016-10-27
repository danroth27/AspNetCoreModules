using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Builder.Internal;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Modules;
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

        public static void AddModules(this IServiceCollection services, Action<ModulesOptions> configureOptions)
        {
            // Hack to get the hosting services
            services.AddSingleton(GetHostingServices(services));
            services.AddTransient<IModuleLoader, ModuleLoader>();
            services.AddSingleton<IModuleManager, ModuleManager>();
            services.Configure(configureOptions);

        }

        static IServiceCollection GetHostingServices(IServiceCollection services)
        {
            var hostingServices = new ServiceCollection();
            hostingServices.Add(services);
            return hostingServices;
        }

        public static void UseModules(this IApplicationBuilder app)
        {
            var moduleManager = app.ApplicationServices.GetService<IModuleManager>();
            var moduleDescriptors = moduleManager.GetModules();
            foreach (var moduleDescriptor in moduleDescriptors)
            {
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

        static IApplicationBuilder GetModuleBuilder(
            IApplicationBuilder app, 
            ModuleDescriptor moduleDescriptor)
        {
            var moduleStartup = moduleDescriptor.ModuleServices.GetRequiredService<IModuleStartup>();
            var moduleBuilder = new ApplicationBuilder(moduleDescriptor.ModuleServices, app.ServerFeatures);
            return moduleBuilder.UseRequestServices(moduleStartup.Configure);
        }

    }
}
