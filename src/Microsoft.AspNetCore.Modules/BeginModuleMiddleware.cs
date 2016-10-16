using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Modules
{
    public class BeginModuleMiddleware
    {
        readonly RequestDelegate _next;
        readonly ModuleOptions _options;      
        IServiceScopeFactory _scopeFactory;

        public BeginModuleMiddleware(RequestDelegate next, ModuleOptions options, IServiceScopeFactory scopeFactory)
        {
            if (next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (scopeFactory == null)
            {
                throw new ArgumentNullException(nameof(scopeFactory));
            }

            _next = next;
            _options = options;
            _scopeFactory = scopeFactory;
        }

        /// <summary>
        /// Executes the middleware.
        /// </summary>
        /// <param name="context">The <see cref="HttpContext"/> for the current request.</param>
        /// <returns>A task that represents the execution of this middleware.</returns>
        public async Task Invoke(HttpContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var beginModule = new BeginModuleFeature();
            beginModule.OriginalRequestServices = context.RequestServices;
            context.RequestServices = _scopeFactory.CreateScope().ServiceProvider;
            var httpContextAccessor = context.RequestServices.GetService<IHttpContextAccessor>();
            if (httpContextAccessor != null)
            {
                httpContextAccessor.HttpContext = context;
            }
            // TODO: How to ensure the module request servces get disposed?

            PathString matchedPath;
            PathString remainingPath;

            if (context.Request.Path.StartsWithSegments(_options.PathBase, out matchedPath, out remainingPath))
            {
                beginModule.PathBaseMatched = true;
                beginModule.OriginalPath = context.Request.Path;
                beginModule.OriginalPathBase = context.Request.PathBase;
                context.Request.Path = remainingPath;
                context.Request.PathBase = beginModule.OriginalPathBase.Add(matchedPath);

                try
                {
                    context.Features.Set<IBeginModuleFeature>(beginModule);
                    await _next(context);
                }
                finally
                {
                    context.Request.Path = beginModule.OriginalPath;
                    context.Request.PathBase = beginModule.OriginalPathBase;
                    context.RequestServices = beginModule.OriginalRequestServices;
                }
            }
            else
            {
                try
                {
                    context.Features.Set<IBeginModuleFeature>(beginModule);
                    await _next(context);
                }
                finally
                {
                    context.RequestServices = beginModule.OriginalRequestServices;
                }
            }            
        }
    }
}
