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

        public static void AddModules(this IServiceCollection services)
        {
            // Hack to get the hosting services
            services.AddSingleton(GetHostingServices(services));

            services.AddTransient<IModuleLoader, ModuleLoader>();
            services.AddSingleton<IModuleManager, ModuleManager>();
        }

        public static void AddModules(this IServiceCollection services, Action<ModulesOptions> configureOptions)
        {
            services.AddModules();
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
