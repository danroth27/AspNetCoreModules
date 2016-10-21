using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Modules.Mvc
{
    public static class ModulesMvcBuilderExtensions
    {
        public static void AddModuleViewOverrides(this IMvcBuilder mvcBuilder)
        {
            mvcBuilder.Services.AddTransient<IConfigureOptions<RazorViewEngineOptions>, ModulesRazorViewEngineOptionsSetup>();
        }
    }
}
