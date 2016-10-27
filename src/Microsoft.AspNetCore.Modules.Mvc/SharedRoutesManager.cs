using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Modules.Mvc
{
    public class SharedRoutesManager : ISharedRoutesManager
    {
        IDictionary<string, IRouteBuilder> _sharedRoutes = new ConcurrentDictionary<string, IRouteBuilder>();

        public IRouteBuilder GetRoutes(string name)
        {
            IRouteBuilder routes;
            _sharedRoutes.TryGetValue(name, out routes);
            return routes;
        }

        public void ShareRoutes(string name, IRouteBuilder routes)
        {
            _sharedRoutes[name] = routes;
        }
    }
}
