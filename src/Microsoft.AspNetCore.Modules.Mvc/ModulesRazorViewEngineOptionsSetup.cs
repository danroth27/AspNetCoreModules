using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Modules.Abstractions;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using System.IO;

namespace Microsoft.AspNetCore.Modules.Mvc
{
    public class ModulesRazorViewEngineOptionsSetup : IConfigureOptions<RazorViewEngineOptions>
    {
        IHostingEnvironment _env;
        IHostingEnvironment _moduleEnv;

        public ModulesRazorViewEngineOptionsSetup(ISharedServiceProvider sharedServices, IHostingEnvironment moduleEnv)
        {
            _env = sharedServices.GetService<IHostingEnvironment>();
            _moduleEnv = moduleEnv;
        }

        public void Configure(RazorViewEngineOptions options)
        {
            //options.FileProviders.Insert(0, GetAppLayoutFileProvider(_env));
            options.FileProviders.Insert(0, GetModuleViewOverridesFileProvider(_env, _moduleEnv));

        }

        static IFileProvider GetAppLayoutFileProvider(IHostingEnvironment _env)
        {
            throw new NotImplementedException(nameof(GetAppLayoutFileProvider));
        }

        static IFileProvider GetModuleViewOverridesFileProvider(IHostingEnvironment env, IHostingEnvironment moduleEnv)
        {
            return new PhysicalFileProvider(Path.Combine(env.ContentRootPath, "Modules", moduleEnv.ApplicationName));
        }
    }
}
