using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Modules.Abstractions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Modules
{
    public class TemplateManager : ITemplateManager
    {
        IDictionary<string, Func<object, Task<IHtmlContent>>> _templates = new ConcurrentDictionary<string, Func<object, Task<IHtmlContent>>>();

        public Task<IHtmlContent> RenderTemplateAsync(string templateName, object model)
        {
            Func<object, Task<IHtmlContent>> template;
            if (_templates.TryGetValue(templateName, out template))
            {
                return template(model);
            }
            return Task.FromResult<IHtmlContent>(null);
        }

        public void AddTemplate(string templateName, Func<object, Task<IHtmlContent>> template)
        {
            _templates[templateName] = template;
        }
    }
}
