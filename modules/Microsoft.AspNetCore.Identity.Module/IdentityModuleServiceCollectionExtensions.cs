using Microsoft.AspNetCore.Modules;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Identity.Module
{
    public static class IdentityModuleServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureIdentityModule(this IServiceCollection services, IConfiguration config)
        {
            var moduleName = typeof(IdentityModuleServiceCollectionExtensions).GetTypeInfo().Assembly.GetName().Name;
            services.ConfigureModule<IdentityModuleOptions>(moduleName, config);
            services.AddForModule(moduleName, moduleServices =>
            {
                moduleServices.Configure<IdentityModuleOptions>(options =>
                {
                    options.ConnectionString = config.GetConnectionString("DefaultConnection");
                });
            });
            return services;
        }
    }
}
