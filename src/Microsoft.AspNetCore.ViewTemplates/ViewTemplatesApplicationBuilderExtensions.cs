using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Html;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Modules.Abstractions;

namespace Microsoft.AspNetCore.ViewTemplates
{
    public static class ViewTemplatesApplicationBuilderExtensions
    {
        public static void ShareTemplate(this IApplicationBuilder app, string templateName, Func<object, Task<IHtmlContent>> template)
        {
            var sharedServices = app.ApplicationServices.GetService<ISharedServiceProvider>();
            var templateManager = sharedServices?.GetService<IViewTemplateManager>();
            templateManager?.AddTemplate(templateName, template);
        }
    }
}
