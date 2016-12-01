using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Modules.Mvc
{
    public class ModulesViewOverridesFeatureProvider : IApplicationFeatureProvider<ViewsFeature>
    {
        IHostingEnvironment _env;
        IRootServiceProvider _rootServices;

        public ModulesViewOverridesFeatureProvider(IHostingEnvironment env, IRootServiceProvider rootServices)
        {
            _env = env;
            _rootServices = rootServices;
        }

        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ViewsFeature feature)
        {
            foreach (var viewOverride in GetFileProviderBasedViewOverrides())
            {
                feature.Views.Remove(viewOverride);
            }
        }

        IEnumerable<string> GetFileProviderBasedViewOverrides()
        {
            var rootEnv = _rootServices.GetRequiredService<IHostingEnvironment>();
            var viewOverridesPath = Path.Combine(rootEnv.ContentRootPath, "Modules", _env.ApplicationName, "Views");
            var viewOverridesDir = rootEnv.ContentRootFileProvider.GetDirectoryContents(viewOverridesPath);

            var dirs = new Stack<IDirectoryContents>();
            dirs.Push(viewOverridesDir);

            while (dirs.Any())
            {
                var dir = dirs.Pop();

                foreach (var fileInfo in dir)
                {
                    if (fileInfo.IsDirectory)
                    {
                        dirs.Push(rootEnv.ContentRootFileProvider.GetDirectoryContents(fileInfo.PhysicalPath));
                    }
                    else
                    {
                        if (fileInfo.Name.EndsWith(".cshtml"))
                        {
                            yield return fileInfo.PhysicalPath;
                        }
                    }
                }
            }
        }
    }
}
