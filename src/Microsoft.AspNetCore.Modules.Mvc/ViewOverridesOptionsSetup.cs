using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Modules.Mvc
{
    public class ViewOverridesOptionsSetup : IConfigureOptions<RazorViewEngineOptions>
    {
        IHostingEnvironment _env;
        IRootServiceProvider _rootServices;

        public ViewOverridesOptionsSetup(IHostingEnvironment env, IRootServiceProvider rootServices)
        {
            _env = env;
            _rootServices = rootServices;
        }

        public void Configure(RazorViewEngineOptions options)
        {
            var rootEnv = _rootServices.GetRequiredService<IHostingEnvironment>();
            var viewOverridesPath = Path.Combine(rootEnv.ContentRootPath, "Modules", _env.ApplicationName);
            options.FileProviders.Insert(0, new PhysicalFileProvider(viewOverridesPath));
        }
    }
}
