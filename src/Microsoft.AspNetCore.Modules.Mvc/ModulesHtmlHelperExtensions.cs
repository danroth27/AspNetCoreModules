using Microsoft.AspNetCore.Modules.Abstractions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Html;

namespace Microsoft.AspNetCore.Modules.Mvc
{
    public static class ModulesHtmlHelperExtensions
    {
        public static Task<IHtmlContent> ModulePartialAsync(this IHtmlHelper htmlHelper, string partialViewName, string moduleName)
        {
            return ModulePartialAsync(htmlHelper, partialViewName, moduleName, htmlHelper.ViewData.Model, viewData: null);
        }

        public static Task<IHtmlContent> ModulePartialAsync(this IHtmlHelper htmlHelper, string partialViewName, string moduleName, object model, ViewDataDictionary viewData)
        {
            var viewContext = htmlHelper.ViewContext;
            var moduleManager = viewContext.HttpContext.RequestServices.GetService<IModuleManager>();
            var module = moduleManager.GetModule(moduleName);
            var moduleHtmlHelper = module.ModuleServices.GetService<IHtmlHelper>();
            var moduleViewContext = new ViewContext(viewContext, viewContext.View, viewContext.ViewData, viewContext.Writer);
            moduleViewContext.HttpContext = new ModuleHttpContext(viewContext.HttpContext, module.ModuleServices);
            (moduleHtmlHelper as IViewContextAware)?.Contextualize(moduleViewContext);
            return moduleHtmlHelper.PartialAsync(partialViewName, model, viewData);
        }

        public static Task<IHtmlContent> ModulePartialAsync(this IHtmlHelper htmlHelper, string partialViewName)
        {
            return ModulePartialAsync(htmlHelper, partialViewName, htmlHelper.ViewData.Model, viewData: null);
        }

        public static async Task<IHtmlContent> ModulePartialAsync(this IHtmlHelper htmlHelper, string partialViewName, object model, ViewDataDictionary viewData)
        {
            var viewContext = htmlHelper.ViewContext;
            var moduleManager = viewContext.HttpContext.RequestServices.GetService<IModuleManager>();
            
            // TODO: Add real partial view discovery logic here that doesn't rely on exception handling
            try
            {
                return await htmlHelper.PartialAsync(partialViewName, model, viewData);
            }
            catch (InvalidOperationException)
            {
                foreach (var module in moduleManager.GetModules())
                {
                    try
                    {
                        return await htmlHelper.ModulePartialAsync(partialViewName, module.Name, model, viewData);
                    }
                    catch (InvalidOperationException)
                    {
                    }
                }
                throw;
            }
        }

        public static IHtmlContent ModulePartial(this IHtmlHelper htmlHelper, string partialViewName, string moduleName)
        {
            var result = ModulePartialAsync(htmlHelper, partialViewName, moduleName);
            return result.GetAwaiter().GetResult();
        }

        public static IHtmlContent ModulePartial(this IHtmlHelper htmlHelper, string partialViewName)
        {
            var result = ModulePartialAsync(htmlHelper, partialViewName);
            return result.GetAwaiter().GetResult();
        }
    }

}
