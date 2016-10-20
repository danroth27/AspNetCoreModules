using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Http.Features;
using System.Security.Claims;
using System.Threading;

namespace Microsoft.AspNetCore.Modules
{
    public class ModuleHttpContext : HttpContext
    {
        HttpContext _context;
        IServiceProvider _moduleServiceProvider;

        public ModuleHttpContext(HttpContext context, IServiceProvider moduleServiceProvider)
        {
            _context = context;
            _moduleServiceProvider = moduleServiceProvider;
        }

        public override AuthenticationManager Authentication
        {
            get
            {
                return _context.Authentication;
            }
        }

        public override ConnectionInfo Connection
        {
            get
            {
                return _context.Connection;
            }
        }

        public override IFeatureCollection Features
        {
            get
            {
                return _context.Features;
            }
        }

        public override IDictionary<object, object> Items
        {
            get
            {
                return _context.Items;
            }

            set
            {
                _context.Items = value;
            }
        }

        public override HttpRequest Request
        {
            get
            {
                return _context.Request;
            }
        }

        public override CancellationToken RequestAborted
        {
            get
            {
                return _context.RequestAborted;
            }

            set
            {
                _context.RequestAborted = value;
            }
        }

        public override IServiceProvider RequestServices
        {
            get
            {
                return _moduleServiceProvider ?? _context.RequestServices;
            }

            set
            {
                _moduleServiceProvider = null;
                _context.RequestServices = value;
            }
        }

        public override HttpResponse Response
        {
            get
            {
                return _context.Response;
            }
        }

        public override ISession Session
        {
            get
            {
                return _context.Session;
            }

            set
            {
                _context.Session = value;
            }
        }

        public override string TraceIdentifier
        {
            get
            {
                return _context.TraceIdentifier;
            }
            set
            {
                _context.TraceIdentifier = value;
            }
        }

        public override ClaimsPrincipal User
        {
            get
            {
                return _context.User;
            }
            set
            {
                _context.User = value;
            }
        }

        public override WebSocketManager WebSockets
        {
            get
            {
                return _context.WebSockets;
            }
        }

        public override void Abort()
        {
            _context.Abort();
        }
    }
}
