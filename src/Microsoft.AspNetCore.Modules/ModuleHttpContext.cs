using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Modules
{
    public class ModuleHttpContext : DefaultHttpContext
    {
        public ModuleHttpContext(IFeatureCollection features, IServiceProvider requestServices)
            : this(features, requestServices, pathBase: null)
        {
        }

        public ModuleHttpContext(IFeatureCollection features, IServiceProvider requestServices, PathString pathBase)
            : base(new FeatureCollection(features))
        {
            Features[typeof(IServiceProvidersFeature)] = new ServiceProvidersFeature() { RequestServices = requestServices };
            var httpRequestFeature = Features.Get<IHttpRequestFeature>();
            Features[typeof(IHttpRequestFeature)] = new HttpRequestFeature()
            {
                Body = httpRequestFeature.Body,
                Headers = httpRequestFeature.Headers,
                Method = httpRequestFeature.Method,
                Path = httpRequestFeature.Path,
                PathBase = new PathString(httpRequestFeature.PathBase).Add(pathBase),
                Protocol = httpRequestFeature.Protocol,
                QueryString = httpRequestFeature.QueryString,
                RawTarget = httpRequestFeature.RawTarget,
                Scheme = httpRequestFeature.Scheme
            };
            Features[typeof(IItemsFeature)] = new ItemsFeature();
        }
    }
}
