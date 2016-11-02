using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Modules.Mvc
{
    public static class MvcBuilderModulesExtensions
    {
        public static IMvcBuilder IgnoreModules(this IMvcBuilder mvcBuilder)
        {
            mvcBuilder.ConfigureApplicationPartManager(appPartManager =>
            {
                var moduleParts = appPartManager.ApplicationParts.Where(part => part.Name.IndexOf("Module", StringComparison.OrdinalIgnoreCase) != -1).ToList();
                foreach (var appPart in moduleParts)
                {
                    appPartManager.ApplicationParts.Remove(appPart);
                }
            });

            return mvcBuilder;
        }
    }
}
