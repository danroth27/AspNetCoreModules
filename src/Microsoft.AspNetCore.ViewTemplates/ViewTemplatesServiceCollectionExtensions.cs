using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.ViewTemplates
{
    public static class ViewTemplatesServiceCollectionExtensions
    {
        public static void AddViewTemplates(this IServiceCollection services)
        {
            services.AddSingleton<IViewTemplateManager>(new ViewTemplateManager());
        }
    }
}
