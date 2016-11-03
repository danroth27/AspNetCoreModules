using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Modules
{
    public class ModuleInstanceIdProvider
    {
        public ModuleInstanceIdProvider(string moduleInstanceId)
        {
            ModuleInstanceId = moduleInstanceId;
        }

        public string ModuleInstanceId { get; }
    }
}
