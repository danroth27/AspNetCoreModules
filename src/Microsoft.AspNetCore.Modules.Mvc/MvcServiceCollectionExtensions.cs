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
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;

namespace Microsoft.AspNetCore.Modules.Mvc
{
    public static class MvcServiceCollectionExtensions
    {
        public static IServiceCollection AddViewOverrides(this IServiceCollection services)
        {
            services.AddTransient<IRazorPageFactoryProvider, ModulesRazorPageFactoryProvider>();
            return services;
        }
    }
}
