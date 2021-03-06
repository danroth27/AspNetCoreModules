﻿using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Microsoft.AspNetCore.Modules
{
    public class ModuleOptions
    {
        public IList<Action<IServiceCollection>> ConfigureServices { get; } = new List<Action<IServiceCollection>>();
    }
}