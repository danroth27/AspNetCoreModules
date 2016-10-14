using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Modules
{
    public class RequestNotHandledMiddleware
    {
        public RequestNotHandledMiddleware(RequestDelegate next)
        {
        }

        public Task Invoke(HttpContext context)
        {
            context.Features.Set<IRequestNotHandledFeature>(new RequestNotHandledFeature() { Handled = false });
            return Task.CompletedTask;
        }
    }

    public interface IRequestNotHandledFeature
    {
        bool Handled { get; set; }
    }

    public class RequestNotHandledFeature : IRequestNotHandledFeature
    {
        public bool Handled { get; set; }
    }
}
