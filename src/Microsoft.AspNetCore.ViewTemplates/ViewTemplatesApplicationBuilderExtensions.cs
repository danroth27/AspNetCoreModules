using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Html;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.ViewTemplates
{
    public static class ViewTemplatesApplicationBuilderExtensions
    {
        public static void ShareTemplate(this IApplicationBuilder app, string templateName, Func<object, Task<IHtmlContent>> template)
        {
            var templateManager = app.ApplicationServices.GetService<IViewTemplateManager>();
            templateManager?.AddTemplate(templateName, template);
        }
    }
}
