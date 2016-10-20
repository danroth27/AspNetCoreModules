using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Modules.Abstractions;
using Microsoft.AspNetCore.Modules;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;

namespace Microsoft.AspNetCore.Modules.Mvc
{
    public class ModulesHtmlHelpers : IViewContextAware
    {
        IModuleManager _moduleManager;
        ViewContext _viewContext;

        public ModulesHtmlHelpers(IModuleManager moduleManager)
        {
            _moduleManager = moduleManager;
        }

        public void Contextualize(ViewContext viewContext)
        {
            _viewContext = viewContext;
        }

        public Task<IHtmlContent> PartialAsync(string partialViewName, string moduleName)
        {
            return PartialAsync(partialViewName, moduleName, _viewContext.ViewData.Model, viewData: null);
        }

        public Task<IHtmlContent> PartialAsync(string partialViewName, string moduleName, object model, ViewDataDictionary viewData)
        {
            var module = _moduleManager.GetModule(moduleName);
            var moduleHtmlHelper = module.ModuleServices.GetService<IHtmlHelper>();
            var moduleViewContext = new ViewContext(_viewContext, _viewContext.View, _viewContext.ViewData, _viewContext.Writer);
            moduleViewContext.HttpContext = new ModuleHttpContext(_viewContext.HttpContext, module.ModuleServices);
            (moduleHtmlHelper as IViewContextAware)?.Contextualize(moduleViewContext);
            return moduleHtmlHelper.PartialAsync(partialViewName, model, viewData);
        }
    }
}
