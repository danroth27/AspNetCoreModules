using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.PlatformAbstractions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Modules
{
    public class DynamicModuleLoader : IModuleLoader
    {
        public DynamicModuleLoader()
        {
            AssemblyLoadContext.Default.Resolving += ResolveModuleAssemby;
        }

        public Assembly ResolveModuleAssemby(AssemblyLoadContext context, AssemblyName assemblyName)
        {
            var assemblyPaths = GetModuleDirs()
                .SelectMany(moduleDir => Directory.EnumerateFiles(moduleDir, $"{assemblyName.Name}.dll"));

            AssemblyName liftedAssemblyName = null;
            foreach (var path in assemblyPaths)
            {
                var moduleAssemblyName = AssemblyLoadContext.GetAssemblyName(path);
                if (liftedAssemblyName == null || moduleAssemblyName.Version > liftedAssemblyName.Version)
                {
                    liftedAssemblyName = moduleAssemblyName;
                }
            }

            return liftedAssemblyName != null ? context.LoadFromAssemblyName(liftedAssemblyName) : null;
        }

        AssemblyName LiftAssemblyVersion(AssemblyName assemblyName1, AssemblyName assemblyName2)
        {
            return assemblyName1.Version > assemblyName2.Version ? assemblyName1 : assemblyName2;
        }

        public IEnumerable<ModuleDescriptor> GetModuleDescriptors(ModuleLoadContext context)
        {
            return GetModuleDirs().Where(HasDynamicModule).Select(moduleDir => GetDynamicModuleDescriptor(moduleDir, context));
        }

        bool HasDynamicModule(string moduleDir)
        {
            var moduleDirInfo = new DirectoryInfo(moduleDir);
            return File.Exists(Path.Combine(moduleDir, moduleDirInfo.Name + ".dll"));
        }

        ModuleDescriptor GetDynamicModuleDescriptor(string moduleDir, ModuleLoadContext context)
        {
            var env = context.HostingEnvironment;
            var moduleAssemblyPath = GetDynamicModuleAssemblyPath(moduleDir);
            var moduleAssembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(moduleAssemblyPath);
            var moduleStartupType = ModuleStartupLoader.FindStartupType(moduleAssembly, env.EnvironmentName);
            return new ModuleDescriptor(moduleStartupType, context.HostingServices, env, context.ModuleOptions);
        }

        string GetDynamicModuleAssemblyPath(string moduleDir)
        {
            var moduleDirInfo = new DirectoryInfo(moduleDir);
            return Path.Combine(moduleDir, $"{moduleDirInfo.Name}.dll");
        }

        public string ModulesPath { get; set; } = Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "Modules");

        IEnumerable<string> GetModuleDirs()
        {
            return Directory.EnumerateDirectories(ModulesPath);
        }
    }
}
