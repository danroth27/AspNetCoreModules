using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Modules
{
    public interface IModuleStartup : IStartup
    {
        IServiceProvider ConfigureSharedServices(IServiceCollection services);
    }
}
