using Microsoft.AspNetCore.Modules.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using System.Reflection;
using System.Runtime.ExceptionServices;

namespace Microsoft.AspNetCore.Modules
{
    public class ConventionBasedModuleStartup : ConventionBasedStartup, IModuleStartup
    {
        ModuleStartupMethods _methods;

        public ConventionBasedModuleStartup(ModuleStartupMethods methods) : base(methods)
        {
            _methods = methods;
        }

        public IServiceProvider ConfigureSharedServices(IServiceCollection services)
        {
            try
            {
                return _methods.ConfigureSharedServicesDelegate(services);
            }
            catch (Exception ex)
            {
                if (ex is TargetInvocationException)
                {
                    ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
                }

                throw;
            }
        }
    }
}
