using Microsoft.AspNetCore.Html;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Modules.Abstractions
{
    public interface ITemplate
    {
        string Name { get; set; }

        Task<IHtmlContent> RenderAsync(object model);
    }
}
