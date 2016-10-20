using Microsoft.AspNetCore.Modules.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Microsoft.AspNetCore.Modules
{
    public class ModulesHtmlHelper : IModulesHtmlHelper, IViewContextAware
    {
        ITemplateManager _templateManager;
        ViewContext _viewContext;

        public ModulesHtmlHelper(ITemplateManager templateManager)
        {
            _templateManager = templateManager;
        }

        public void Contextualize(ViewContext viewContext)
        {
            _viewContext = viewContext;
        }

        public Task<IHtmlContent> TemplateAsync(string templateName)
        {
            return TemplateAsync(templateName, null);
        }

        public Task<IHtmlContent> TemplateAsync(string templateName, object model)
        {
            return _templateManager.RenderTemplateAsync(templateName, model);
        }
    }
}
