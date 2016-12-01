using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using Microsoft.AspNetCore.Mvc.Razor.Internal;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Modules.Mvc
{
    public class ModulesRazorPageFactoryProvider : IRazorPageFactoryProvider
    {
        IRazorPageFactoryProvider _appProvider;
        IRazorPageFactoryProvider _defaultProvider;
        IHostingEnvironment _moduleEnv;
        IHostingEnvironment _rootEnv;

        public ModulesRazorPageFactoryProvider(
            IRazorCompilationService razorCompilationService, 
            ICompilerCacheProvider compilerCacheProvider, 
            IHostingEnvironment moduleEnv,
            IRootServiceProvider rootServices)
        {
            _appProvider = rootServices.GetService<IRazorPageFactoryProvider>();
            if (_appProvider == null)
            {
                throw new ArgumentNullException(nameof(_appProvider));
            }

            _defaultProvider = new DefaultRazorPageFactoryProvider(razorCompilationService, compilerCacheProvider);
            _moduleEnv = moduleEnv;
            _rootEnv = rootServices.GetRequiredService<IHostingEnvironment>();
        }

        public RazorPageFactoryResult CreateFactory(string relativePath)
        {
            var pathString = new PathString(relativePath);
            var viewOverridesPathString = new PathString($"/Modules/{_moduleEnv.ApplicationName}").Add(pathString);
            var result = _appProvider.CreateFactory(viewOverridesPathString);
            if (result.Success)
            {
                return result;
            }

            PathString remainingPath;
            // TODO: Need a better way to determine if the path is for a layout page
            if (pathString.StartsWithSegments("/Views/Shared/_Layout.cshtml", out remainingPath) && 
                remainingPath.Equals(PathString.Empty))
            {
                result = _appProvider.CreateFactory(pathString);
                if (result.Success)
                {
                    return result;
                }
            }

            return _defaultProvider.CreateFactory(relativePath);
        }
    }
}
