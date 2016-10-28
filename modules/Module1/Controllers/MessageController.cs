using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Module1.Controllers
{
    // TODO: When the module is directly reference should this controller be found by the app?
    public class MessageController : Controller
    {
        [HttpGet("api/message")]
        public string GetMessage()
        {
            return "Hello from Web API in Module1!";
        }
    }
}
