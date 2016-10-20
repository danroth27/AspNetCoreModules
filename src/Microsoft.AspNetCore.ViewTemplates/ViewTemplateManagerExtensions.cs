using Microsoft.AspNetCore.Html;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.ViewTemplates
{
    public static class ViewTemplateManagerExtensions
    {
        public static Task<IHtmlContent> RenderTemplateAsync(this IViewTemplateManager templateManager, string templateName)
        {
            return templateManager.RenderTemplateAsync(templateName, null);
        }
    }
}
