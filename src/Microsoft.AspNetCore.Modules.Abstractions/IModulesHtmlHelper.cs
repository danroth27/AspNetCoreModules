using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Modules.Abstractions
{
    public interface IModulesHtmlHelper

    {
        Task<IHtmlContent> TemplateAsync(string templateName);
        Task<IHtmlContent> TemplateAsync(string templateName, object model);
    }
}
