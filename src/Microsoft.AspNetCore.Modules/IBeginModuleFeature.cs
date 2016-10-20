using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Modules
{
    public interface IBeginModuleFeature
    {
        IServiceProvider OriginalRequestServices { get; set; }
    }
}
