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
        public static IServiceCollection AddModules(this IServiceCollection services)
        {
            // Hack to get the hosting services
            services.AddSingleton(GetHostingServices(services));

            services.AddTransient<IModuleLoader, ModuleLoader>();
            services.AddSingleton<IModuleManager, ModuleManager>();

            return services;
        }

        public static IServiceCollection AddModules(this IServiceCollection services, Action<ModulesOptions> configureOptions)
        {
            services.AddModules();
            services.Configure(configureOptions);

            return services;
        }

        public static IServiceCollection AddModules(this IServiceCollection services, IConfiguration config)
        {
            services.AddModules();
            services.Configure<ModulesOptions>(options =>
            {
                foreach (var moduleConfig in config.GetModules().GetChildren())
                {
                    var pathBase = moduleConfig["PathBase"];
                    if (!String.IsNullOrEmpty(pathBase))
                    {
                        options.PathBase[moduleConfig.Key] = pathBase;
                    }
                }
            });

            return services;
        }

        static IServiceCollection GetHostingServices(IServiceCollection services)
        {
            var hostingServices = new ServiceCollection();
            hostingServices.Add(services);
            return hostingServices;
        }

        public static IServiceCollection AddForModule(
            this IServiceCollection services,
            string moduleName,
            Action<IServiceCollection> setupServices)
        {
            return services.AddSingleton<IConfigureModuleServices>(new ConfigureModuleServices(moduleName, setupServices));
        }

        public static IServiceCollection AddForModuleInstance(
            this IServiceCollection services,
            string moduleInstanceId,
            Action<IServiceCollection> setupServices)
        {
            return services.AddSingleton<IConfigureModuleInstanceServices>(new ConfigureModuleInstanceServices(moduleInstanceId, setupServices));
        }

        public static IServiceCollection ConfigureModule<TOptions>(
            this IServiceCollection services,
            string moduleName,
            Action<TOptions> setupOptions)
            where TOptions : class
        {
            return services.AddForModule(moduleName, moduleServices =>
            {
                moduleServices.Configure<TOptions>(setupOptions);
            });
        }

        public static IServiceCollection ConfigureModule<TOptions>(
            this IServiceCollection services,
            string moduleName,
            IConfiguration config)
            where TOptions : class
        {
            return services.AddForModule(moduleName, moduleServices =>
            {
                moduleServices.Configure<TOptions>(config.GetModules().GetSection(moduleName));
            });
        }

        public static IServiceCollection ConfigureModuleInstance<TOptions>(
            this IServiceCollection services,
            string moduleInstanceId,
            Action<TOptions> setupOptions)
            where TOptions : class
        {
            return services.AddForModuleInstance(moduleInstanceId, moduleServices =>
            {
                moduleServices.Configure<TOptions>(setupOptions);
            });
        }

        public static IServiceCollection ConfigureModuleInstance<TOptions>(
            this IServiceCollection services,
            string moduleInstanceId,
            IConfiguration config)
            where TOptions : class
        {
            return services.AddForModuleInstance(moduleInstanceId, moduleServices =>
            {
                moduleServices.Configure<TOptions>(config.GetModuleInstances().GetSection(moduleInstanceId));
            });
        }

        public static IConfigurationSection GetModules(this IConfiguration config)
        {
            return config.GetSection("Modules");
        }

        public static IConfigurationSection GetModuleInstances(this IConfiguration config)
        {
            return config.GetSection("ModuleInstances");
        }

        public static void UseModules(this IApplicationBuilder app)
        {
            var moduleManager = app.ApplicationServices.GetRequiredService<IModuleManager>();
            moduleManager.UseModules(app);
        }

        public static void UseModule(this IApplicationBuilder app, string moduleName)
        {
            app.UseModule(moduleName, moduleName);
        }

        public static void UseModule(this IApplicationBuilder app, string moduleName, string moduleInstanceId)
        {
            app.UseModule(moduleName, moduleInstanceId, PathString.Empty);
        }

        public static void UseModule(this IApplicationBuilder app, string moduleName, string moduleInstanceId, PathString pathBase)
        {
            var moduleManager = app.ApplicationServices.GetService<IModuleManager>();
            moduleManager.UseModule(app, moduleName, moduleInstanceId, pathBase);
        }
    }
}
