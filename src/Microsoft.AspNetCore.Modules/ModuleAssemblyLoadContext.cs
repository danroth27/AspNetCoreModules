using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Modules
{
    public class ModuleAssemblyLoadContext : AssemblyLoadContext
    {
        string _modulePath;

        public ModuleAssemblyLoadContext(string modulePath)
        {
            _modulePath = modulePath;
            Resolving += ModuleAssemblyLoadContext_Resolving;
        }

        private Assembly ModuleAssemblyLoadContext_Resolving(AssemblyLoadContext context, AssemblyName name)
        {
            return LoadFromAssemblyPath(Path.Combine(_modulePath, $"{name.Name}.dll"));
        }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            return null;
        }
    }
}
