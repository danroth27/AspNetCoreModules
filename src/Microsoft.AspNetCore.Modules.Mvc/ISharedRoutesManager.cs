using Microsoft.AspNetCore.Routing;

namespace Microsoft.AspNetCore.Modules.Mvc
{
    public interface ISharedRoutesManager
    {
        IRouteBuilder GetRoutes(string name);
        void ShareRoutes(string name, IRouteBuilder routes);
    }
}