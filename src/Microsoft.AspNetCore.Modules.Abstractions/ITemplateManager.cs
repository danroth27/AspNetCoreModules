using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Modules.Abstractions
{
    public interface ITemplateManager
    {
        Task<IHtmlContent> RenderTemplateAsync(string templateName, object model);

        void AddTemplate(string name, Func<object, Task<IHtmlContent>> template);
    }
}
