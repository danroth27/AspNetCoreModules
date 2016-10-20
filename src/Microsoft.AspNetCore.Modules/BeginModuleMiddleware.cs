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
        IServiceScopeFactory _scopeFactory;

        public BeginModuleMiddleware(RequestDelegate next, IServiceScopeFactory scopeFactory)
        {
            if (next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }

            if (scopeFactory == null)
            {
                throw new ArgumentNullException(nameof(scopeFactory));
            }

            _next = next;
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

            using (var scope = _scopeFactory.CreateScope())
            {
                context.RequestServices = scope.ServiceProvider;
                var httpContextAccessor = context.RequestServices.GetService<IHttpContextAccessor>();
                if (httpContextAccessor != null)
                {
                    httpContextAccessor.HttpContext = context;
                }

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
