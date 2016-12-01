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
        IHostingEnvironment _env;

        public ModulesRazorPageFactoryProvider(
            IRazorCompilationService razorCompilationService, 
            ICompilerCacheProvider compilerCacheProvider, 
            IHostingEnvironment env,
            IRootServiceProvider rootServiceProvider)
        {
            _appProvider = rootServiceProvider.GetService<IRazorPageFactoryProvider>();
            if (_appProvider == null)
            {
                throw new ArgumentNullException(nameof(_appProvider));
            }

            _defaultProvider = new DefaultRazorPageFactoryProvider(razorCompilationService, compilerCacheProvider);
            _env = env;
        }

        public RazorPageFactoryResult CreateFactory(string relativePath)
        {
            var pathString = new PathString(relativePath);
            PathString remainingPath;
            // TODO: Need a better way to determine if the path is for a layout page
            if (pathString.StartsWithSegments("/Views/Shared/_Layout.cshtml", out remainingPath) && 
                remainingPath.Equals(PathString.Empty))
            {
                var result = _appProvider.CreateFactory(pathString);
                if (result.Success)
                {
                    return result;
                }
            }

            return _defaultProvider.CreateFactory(relativePath);
        }
    }
}
