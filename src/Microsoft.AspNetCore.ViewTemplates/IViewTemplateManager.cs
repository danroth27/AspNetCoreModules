using Microsoft.AspNetCore.Html;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.ViewTemplates
{
    public interface IViewTemplateManager
    {
        Task<IHtmlContent> RenderTemplateAsync(string templateName, object model);

        void AddTemplate(string name, Func<object, Task<IHtmlContent>> template);
    }
}
