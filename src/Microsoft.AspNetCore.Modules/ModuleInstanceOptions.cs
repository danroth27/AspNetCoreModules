using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Microsoft.AspNetCore.Modules
{
    public class ModuleInstanceOptions
    {
        public string PathBase { get; set; }

        public IList<Action<IServiceCollection>> ConfigureServices { get; } = new List<Action<IServiceCollection>>();
    }
}