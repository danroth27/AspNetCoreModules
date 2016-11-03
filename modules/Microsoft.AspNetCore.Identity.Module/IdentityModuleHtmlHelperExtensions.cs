using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Modules.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Identity.Module
{
    public static class IdentityModuleHtmlHelperExtensions
    {
        public static Task<IHtmlContent> IdentityLoginPartialAsync(this IHtmlHelper htmlHelper)
        {
            return htmlHelper.ModulePartialAsync("_LoginPartial", "Microsoft.AspNetCore.Identity.Module");
        }
    }
}
